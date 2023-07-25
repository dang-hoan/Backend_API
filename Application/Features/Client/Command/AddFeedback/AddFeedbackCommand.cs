using System.ComponentModel.DataAnnotations;
using Application.Enums;
using Application.Interfaces;
using Application.Interfaces.Booking;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Feedback;
using Application.Interfaces.FeedbackFileUpload;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using AutoMapper;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Application.Features.Client.Command.AddFeedback
{
    public class AddFeedbackCommand : IRequest<Result<AddFeedbackCommand>>
    {
        [Required]
        public string Rating { get; set; }
        [Required]
        public long BookingDetailId { get; set; }
        [Required]
        public long CustomerId { get; set; }
        public string? StaffContent { get; set; }
        public string? ServiceContent { get; set; }
        public List<IFormFile>? ListImages { get; set; }
        public List<IFormFile>? ListVideos { get; set; }

    }
    internal class AddFeedbackCommandHandler : IRequestHandler<AddFeedbackCommand, Result<AddFeedbackCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IFeedbackFileUploadRepository _feedbackFileUploadRepository;
        private readonly ICheckFileType _checkFileType;
        private readonly ICheckSizeFile _checkSizeFile;
        private readonly IUploadService _uploadService;
        private readonly IBookingDetailRepository _bookingDetailRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ILogger<AddFeedbackCommand> _logger;
        public AddFeedbackCommandHandler(
            IMapper mapper, IFeedbackRepository feedbackRepository,
            IFeedbackFileUploadRepository feedbackFileUploadRepository,
            IBookingDetailRepository bookingDetailRepository,
            IUnitOfWork<long> unitOfWork,
            IUploadService uploadService,
            ICheckFileType checkFileType,
            ICheckSizeFile checkSizeFile,
            ILogger<AddFeedbackCommand> logger
         )
        {
            _mapper = mapper;
            _feedbackRepository = feedbackRepository;
            _feedbackFileUploadRepository = feedbackFileUploadRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _checkFileType = checkFileType;
            _checkSizeFile = checkSizeFile;
            _uploadService = uploadService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<AddFeedbackCommand>> Handle(AddFeedbackCommand request, CancellationToken cancellationToken)
        {
            var isExistInBookingDetail = await _bookingDetailRepository.FindAsync(_ => _.Id == request.BookingDetailId && _.IsDeleted == false) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_BOOKING_DETAIL);
            var addFeedback = _mapper.Map<Domain.Entities.Feedback.Feedback>(request);
            await _feedbackRepository.AddAsync(addFeedback);
            await _unitOfWork.Commit(cancellationToken);

            if (request.ListImages != null)
            {
                var imagesResult = _checkFileType.CheckFilesIsImage(new Dtos.Requests.CheckImagesTypeRequest
                {
                    Files = request.ListImages
                });
                var imageCheckMaxSize = _checkSizeFile.CheckImageSize(new Dtos.Requests.CheckImageSizeRequest
                {
                    Files = request.ListImages
                });
                
                if (imagesResult != "")
                    return await Result<AddFeedbackCommand>.FailAsync(imagesResult);
                
                if (imageCheckMaxSize != "")
                    return await Result<AddFeedbackCommand>.FailAsync(imageCheckMaxSize);

                foreach (IFormFile file in request.ListImages)
                {
                    string uploadResult = await UploadFile(file, addFeedback.Id, Enums.UploadType.ProfilePicture);
                    if (uploadResult != "")
                        return await Result<AddFeedbackCommand>.FailAsync(uploadResult);
                }
            }

            if (request.ListVideos != null)
            {
                var videosResult = _checkFileType.CheckFilesIsVideo(new Dtos.Requests.CheckVideoTypeRequest
                {
                    Files = request.ListVideos
                });
                var videoCheckMaxSize = _checkSizeFile.CheckVideoSize(new Dtos.Requests.CheckVideoSizeRequest
                {
                    Files = request.ListVideos
                });
                if (videosResult != "")
                    return await Result<AddFeedbackCommand>.FailAsync(videosResult);

                if (videoCheckMaxSize != "")
                    return await Result<AddFeedbackCommand>.FailAsync(videoCheckMaxSize);

                foreach (IFormFile file in request.ListVideos)
                {
                    string uploadResult = await UploadFile(file, addFeedback.Id, Enums.UploadType.UploadVideo);
                    if (uploadResult != "")
                        return await Result<AddFeedbackCommand>.FailAsync(uploadResult);
                }
            }

            await _unitOfWork.Commit(cancellationToken);

            return await Result<AddFeedbackCommand>.SuccessAsync(request);

        }

        public async Task<string> UploadFile(IFormFile file, long targetId, UploadType uploadTypes)
        {
            string resultFileUpload = "";

            var filePath = _uploadService.UploadAsync(new Dtos.Requests.UploadRequest
            {
                FileName = file.FileName,
                Extension = Path.GetExtension(file.FileName),
                UploadType = uploadTypes,
                Data = _mapper.Map<byte[]>(file)
            });

            if (filePath != "")
            {
                var obj = new Domain.Entities.FeebackFileUpload.FeedbackFileUpload
                {
                    FeedbackId = targetId,
                    NameFile = filePath,
                    TypeFile = (uploadTypes == UploadType.ProfilePicture) ? "Image" : "Video"
                };
                await _feedbackFileUploadRepository.AddAsync(obj);
            }
            else
            {
                resultFileUpload = file.FileName;
                return resultFileUpload;
            }
            return resultFileUpload;
        }
    }
}

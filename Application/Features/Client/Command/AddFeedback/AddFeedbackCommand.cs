using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application.Dtos.Responses.FeedbackFileUpload;
using Application.Enums;
using Application.Interfaces;
using Application.Interfaces.BookingDetail;
using Application.Interfaces.Feedback;
using Application.Interfaces.FeedbackFileUpload;
using Application.Interfaces.Repositories;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Domain.Constants.Enum;
using Application.Interfaces.Booking;

namespace Application.Features.Client.Command.AddFeedback
{
    public class AddFeedbackCommand : IRequest<Result<AddFeedbackCommand>>
    {
        [Required]
        public string Rating { get; set; }
        [Required]
        public long BookingDetailId { get; set; }
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
        private readonly IBookingRepository _bookingRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ILogger<AddFeedbackCommand> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public AddFeedbackCommandHandler(
            IMapper mapper, IFeedbackRepository feedbackRepository,
            IFeedbackFileUploadRepository feedbackFileUploadRepository,
            IBookingDetailRepository bookingDetailRepository,
            IBookingRepository bookingRepository,
            IUnitOfWork<long> unitOfWork,
            IUploadService uploadService,
            ICheckFileType checkFileType,
            ICheckSizeFile checkSizeFile,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AddFeedbackCommand> logger,
            UserManager<AppUser> userManager
         )
        {
            _mapper = mapper;
            _feedbackRepository = feedbackRepository;
            _feedbackFileUploadRepository = feedbackFileUploadRepository;
            _bookingDetailRepository = bookingDetailRepository;
            _bookingRepository = bookingRepository;
            _checkFileType = checkFileType;
            _checkSizeFile = checkSizeFile;
            _uploadService = uploadService;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;

        }

        public async Task<Result<AddFeedbackCommand>> Handle(AddFeedbackCommand request, CancellationToken cancellationToken)
        {
            var isExistInBookingDetail = await _bookingDetailRepository.FindAsync(_ => _.Id == request.BookingDetailId && _.IsDeleted == false) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_BOOKING_DETAIL);
            var isExistBookingFeedback = await _feedbackRepository.FindAsync(_ => _.BookingDetailId == request.BookingDetailId && _.IsDeleted == false);
            
            if (isExistBookingFeedback != null)
            {
                return await Result<AddFeedbackCommand>.FailAsync("You rated this feedback");
            }
            var addFeedback = _mapper.Map<Domain.Entities.Feedback.Feedback>(request);
            var userName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customerId = (long)0;
            var currentLoginUser = new AppUser();
            if (userName != null)
            {
                currentLoginUser = await _userManager.FindByNameAsync(userName);
                if (currentLoginUser.TypeFlag != TypeFlagEnum.Customer)
                    return await Result<AddFeedbackCommand>.FailAsync("You are not customer");

                var isCustomerHasBooking = _bookingRepository.Entities.Where(x => x.CustomerId == currentLoginUser.UserId && x.Id == isExistInBookingDetail.BookingId && x.Status == BookingStatus.Done).Select(x => x.Id);
                customerId = currentLoginUser.UserId;
                if (isCustomerHasBooking.Count() == 0)
                    return await Result<AddFeedbackCommand>.FailAsync("You cannot leave feedback of other customer");
            }
            else
            {
                return await Result<AddFeedbackCommand>.FailAsync(StaticVariable.IS_NOT_LOGIN);
            }
            _logger.LogInformation($"log: {JsonConvert.SerializeObject(customerId)}");
            // Check Image is valid
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
            }
            // Check Video is valid
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
            }
            addFeedback.CustomerId = customerId;
            await _feedbackRepository.AddAsync(addFeedback);
            await _unitOfWork.Commit(cancellationToken);

            if (request.ListImages != null)
            {
                foreach (IFormFile file in request.ListImages)
                {
                    string uploadResult = await UploadFile(file, addFeedback.Id, Enums.UploadType.ProfilePicture);
                    if (uploadResult != "")
                        return await Result<AddFeedbackCommand>.FailAsync(uploadResult);
                }
            }
            if (request.ListVideos != null)
            {
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

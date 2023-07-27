using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using AutoMapper;
using Domain.Constants;
using Domain.Entities.ServiceImage;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;


namespace Application.Features.Service.Command.EditService
{
    public class EditServiceCommand : IRequest<Result<EditServiceCommand>>
    {
        public long Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public List<IFormFile> ListImages { get; set; }

        [Required]
        public int ServiceTime { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string? Description { get; set; }
    }

    internal class EditServiceCommandHandler : IRequestHandler<EditServiceCommand, Result<EditServiceCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceImageRepository _serviceImageRepository;
        private readonly ICheckFileType _checkFileType;
        private readonly ICheckSizeFile _checkSizeFile;
        private readonly IUploadService _uploadService;
        private readonly IRemoveImageService _removeImageService;
        private readonly IUnitOfWork<long> _unitOfWork;

        public EditServiceCommandHandler(IMapper mapper, IServiceRepository serviceRepository, IServiceImageRepository serviceImageRepository,
            ICheckFileType checkFileType, IUploadService uploadService, IRemoveImageService removeImageService, IUnitOfWork<long> unitOfWork, ICheckSizeFile checkSizeFile)
        {
            _mapper = mapper;
            _serviceRepository = serviceRepository;
            _serviceImageRepository = serviceImageRepository;
            _checkFileType = checkFileType;
            _checkSizeFile = checkSizeFile;
            _uploadService = uploadService;
            _removeImageService = removeImageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<EditServiceCommand>> Handle(EditServiceCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                return await Result<EditServiceCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }
            var editService = await _serviceRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_MSG);

            var result = _checkFileType.CheckFilesIsImage(new Dtos.Requests.CheckImagesTypeRequest
            {
                Files = request.ListImages
            });
            var imageCheckMaxSize = _checkSizeFile.CheckImageSize(new Dtos.Requests.CheckImageSizeRequest
            {
                Files = request.ListImages
            });

            if (result != "")
                return await Result<EditServiceCommand>.FailAsync(result);

            if (imageCheckMaxSize != "")
                return await Result<EditServiceCommand>.FailAsync(imageCheckMaxSize);

            _mapper.Map(request, editService);
            await _serviceRepository.UpdateAsync(editService);

            IReadOnlyList<ServiceImage> oldServiceImages = await _serviceImageRepository.GetByCondition(serviceImage => serviceImage.ServiceId == request.Id);

            await _serviceImageRepository.DeleteRange(oldServiceImages.ToList());
            foreach (ServiceImage serviceImage in oldServiceImages)
            {
                if (!_removeImageService.RemoveImage(new Dtos.Requests.RemoveImageRequest
                {
                    FilePath = serviceImage.NameFile
                }))
                    return await Result<EditServiceCommand>.FailAsync(StaticVariable.SERVER_ERROR_MSG);
            }

            foreach (IFormFile file in request.ListImages)
            {
                var filePath = _uploadService.UploadAsync(new Dtos.Requests.UploadRequest
                {
                    FileName = file.FileName,
                    Extension = Path.GetExtension(file.FileName),
                    Data = _mapper.Map<byte[]>(file)
                });

                if (filePath != "")
                {
                    var obj = new ServiceImage
                    {
                        ServiceId = editService.Id,
                        NameFile = filePath
                    };
                    await _serviceImageRepository.AddAsync(obj);
                }
                else
                {
                    return await Result<EditServiceCommand>.FailAsync($"File {file.FileName} isn't uploaded!");
                }
            }

            await _unitOfWork.Commit(cancellationToken);

            return await Result<EditServiceCommand>.SuccessAsync(request);
        }
    }
}

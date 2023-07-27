﻿using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using AutoMapper;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Service.Command.AddService
{
    public class AddServiceCommand : IRequest<Result<AddServiceCommand>>
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

    internal class AddServiceCommandHandler : IRequestHandler<AddServiceCommand, Result<AddServiceCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceImageRepository _serviceImageRepository;
        private readonly ICheckFileType _checkFileType;
        private readonly IUploadService _uploadService;
        private readonly ICheckSizeFile _checkSizeFile;
        private readonly IUnitOfWork<long> _unitOfWork;


        public AddServiceCommandHandler(IMapper mapper, IServiceRepository serviceRepository,
            IServiceImageRepository serviceImageRepository, IUnitOfWork<long> unitOfWork, IUploadService uploadService, ICheckFileType checkFileType, ICheckSizeFile checkSizeFile)
        {
            _mapper = mapper;
            _serviceRepository = serviceRepository;
            _serviceImageRepository = serviceImageRepository;
            _checkFileType = checkFileType;
            _checkSizeFile = checkSizeFile;
            _uploadService = uploadService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AddServiceCommand>> Handle(AddServiceCommand request, CancellationToken cancellationToken)
        {
            var result = _checkFileType.CheckFilesIsImage(new Dtos.Requests.CheckImagesTypeRequest
            {
                Files = request.ListImages
            });
            var imageCheckMaxSize = _checkSizeFile.CheckImageSize(new Dtos.Requests.CheckImageSizeRequest
            {
                Files = request.ListImages
            });
            if (result != "")
                return await Result<AddServiceCommand>.FailAsync(result);

            if (imageCheckMaxSize != "")
                return await Result<AddServiceCommand>.FailAsync(imageCheckMaxSize);

            var addService = _mapper.Map<Domain.Entities.Service.Service>(request);
            await _serviceRepository.AddAsync(addService);

            await _unitOfWork.Commit(cancellationToken);
            request.Id = addService.Id;

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
                    var obj = new Domain.Entities.ServiceImage.ServiceImage
                    {
                        ServiceId = addService.Id,
                        NameFile = filePath
                    };
                    await _serviceImageRepository.AddAsync(obj);
                }
                else
                {
                    return await Result<AddServiceCommand>.FailAsync($"File {file.FileName} isn't uploaded!");
                }
            }

            await _unitOfWork.Commit(cancellationToken);

            return await Result<AddServiceCommand>.SuccessAsync(request);
        }

    }
}

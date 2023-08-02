using Application.Dtos.Requests.Feedback;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using AutoMapper;
using Domain.Constants;
using Domain.Entities.ServiceImage;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Service.Command.EditService
{
    public class EditServiceCommand : IRequest<Result<EditServiceCommand>>
    {
        public long Id { get; set; }

        public string Name { get; set; } = default!;
        public List<ImageRequest>? ServicesImageRequests { get; set; } // Hình ảnh mô tả (File)
        public int ServiceTime { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public string? Description { get; set; }
    }

    internal class EditServiceCommandHandler : IRequestHandler<EditServiceCommand, Result<EditServiceCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceImageRepository _serviceImageRepository;
        private readonly IRemoveImageService _removeImageService;
        private readonly IUnitOfWork<long> _unitOfWork;

        public EditServiceCommandHandler(IMapper mapper, IServiceRepository serviceRepository, IServiceImageRepository serviceImageRepository, IRemoveImageService removeImageService, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _serviceRepository = serviceRepository;
            _serviceImageRepository = serviceImageRepository;
            _removeImageService = removeImageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<EditServiceCommand>> Handle(EditServiceCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            if (request.Id == 0)
            {
                return await Result<EditServiceCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }
            var editService = await _serviceRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (editService == null) return await Result<EditServiceCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);

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

            if (request.ServicesImageRequests != null)
            {
                var addImage = _mapper.Map<List<ServiceImage>>(request.ServicesImageRequests);
                addImage.ForEach(x => x.ServiceId = editService.Id);
                await _serviceImageRepository.AddRangeAsync(addImage);
            }
            await _unitOfWork.Commit(cancellationToken);
            await transaction.CommitAsync();
            await transaction.DisposeAsync();
            return await Result<EditServiceCommand>.SuccessAsync(request);
        }
    }
}
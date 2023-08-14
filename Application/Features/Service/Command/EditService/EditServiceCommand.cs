using Application.Dtos.Requests.Feedback;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using AutoMapper;
using Domain.Constants;
using Domain.Entities.ServiceImage;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IUploadService _uploadService;

        public EditServiceCommandHandler(IMapper mapper, IServiceRepository serviceRepository, IServiceImageRepository serviceImageRepository, IUnitOfWork<long> unitOfWork, IUploadService uploadService)
        {
            _mapper = mapper;
            _serviceRepository = serviceRepository;
            _serviceImageRepository = serviceImageRepository;
            _unitOfWork = unitOfWork;
            _uploadService = uploadService;
        }

        public async Task<Result<EditServiceCommand>> Handle(EditServiceCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (request.Id == 0)
                {
                    return await Result<EditServiceCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
                }
                var editService = await _serviceRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
                if (editService == null) return await Result<EditServiceCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);

                _mapper.Map(request, editService);
                await _serviceRepository.UpdateAsync(editService);

                if (request.ServicesImageRequests != null)
                {
                    List<string?> listNewFiles = request.ServicesImageRequests.Select(x => x.NameFile).ToList();
                    //Remove and Add List Request Image
                    var requestImages = await _serviceImageRepository.Entities.Where(x => x.ServiceId == editService.Id && !x.IsDeleted).ToListAsync(cancellationToken);
                    if (requestImages.Any())
                    {
                        foreach (var item in requestImages)
                        {
                            if(!listNewFiles.Contains(item.NameFile))
                                await _uploadService.DeleteAsync(item.NameFile);
                        }
                        await _serviceImageRepository.RemoveRangeAsync(requestImages);
                        await _unitOfWork.Commit(cancellationToken);
                    }
                    var image = _mapper.Map<List<ServiceImage>>(request.ServicesImageRequests);
                    var requestImage = image.Select(x =>
                    {
                        x.Id = 0;
                        x.ServiceId = editService.Id;
                        return x;
                    }).ToList();
                    await _serviceImageRepository.AddRangeAsync(requestImage);
                }
                await _unitOfWork.Commit(cancellationToken);
                await transaction.CommitAsync();
                return await Result<EditServiceCommand>.SuccessAsync(request);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new ApiException(ex.Message);
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }
    }
}
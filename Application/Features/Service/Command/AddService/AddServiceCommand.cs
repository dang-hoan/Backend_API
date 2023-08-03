using Application.Dtos.Requests.Feedback;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using AutoMapper;
using Domain.Entities.ServiceImage;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Service.Command.AddService
{
    public class AddServiceCommand : IRequest<Result<AddServiceCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public int ServiceTime { get; set; } = default!;
        public decimal Price { get; set; } = default!;
        public string? Description { get; set; }
        public List<ImageRequest>? ServicesImageRequests { get; set; } // Hình ảnh mô tả (File)
    }

    internal class AddServiceCommandHandler : IRequestHandler<AddServiceCommand, Result<AddServiceCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceImageRepository _serviceImageRepository;
        private readonly IUnitOfWork<long> _unitOfWork;

        public AddServiceCommandHandler(IMapper mapper, IServiceRepository serviceRepository, IServiceImageRepository serviceImageRepository, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _serviceRepository = serviceRepository;
            _serviceImageRepository = serviceImageRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AddServiceCommand>> Handle(AddServiceCommand request, CancellationToken cancellationToken)
        {
            var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var addService = _mapper.Map<Domain.Entities.Service.Service>(request);
                await _serviceRepository.AddAsync(addService);
                await _unitOfWork.Commit(cancellationToken);
                request.Id = addService.Id;

                if (request.ServicesImageRequests != null)
                {
                    var addImage = _mapper.Map<List<ServiceImage>>(request.ServicesImageRequests);
                    addImage.ForEach(x => x.ServiceId = addService.Id);
                    await _serviceImageRepository.AddRangeAsync(addImage);
                    await _unitOfWork.Commit(cancellationToken);
                }

                await transaction.CommitAsync();
                return await Result<AddServiceCommand>.SuccessAsync(request);
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
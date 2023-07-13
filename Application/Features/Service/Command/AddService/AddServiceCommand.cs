using Application.Interfaces.Repositories;
using Application.Interfaces.Service;
using AutoMapper;
using Domain.Wrappers;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Service.Command.AddService
{
    public class AddServiceCommand : IRequest<Result<AddServiceCommand>>
    {
        public long Id { get; set; }
        [Required]
        public string Name { get; set; }
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
        private readonly IUnitOfWork<long> _unitOfWork;


        public AddServiceCommandHandler(IMapper mapper, IServiceRepository serviceRepository, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _serviceRepository = serviceRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AddServiceCommand>> Handle(AddServiceCommand request, CancellationToken cancellationToken)
        {
            var addService = _mapper.Map<Domain.Entities.Service.Service>(request);
            await _serviceRepository.AddAsync(addService);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = addService.Id;

            return await Result<AddServiceCommand>.SuccessAsync(request);
        }
    }
}

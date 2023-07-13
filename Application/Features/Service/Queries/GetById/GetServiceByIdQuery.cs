using Application.Dtos.Responses.ServiceImage;
using Application.Features.Employee.Queries.GetById;
using Application.Interfaces.Employee;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using AutoMapper;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Service.Queries.GetById
{
    public class GetServiceByIdQuery : IRequest<Result<GetServiceByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetServiceByIdQueryHandler : IRequestHandler<GetServiceByIdQuery, Result<GetServiceByIdResponse>>
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IServiceImageRepository _serviceImageRepository;
        private readonly IMapper _mapper;

        public GetServiceByIdQueryHandler(IServiceRepository serviceRepository, IServiceImageRepository serviceImageRepository, IMapper mapper)
        {
            _serviceRepository = serviceRepository;
            _serviceImageRepository = serviceImageRepository;
            _mapper = mapper;
        }

        public async Task<Result<GetServiceByIdResponse>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var service =  _serviceRepository.Entities
                .Where(_ => _.Id == request.Id && _.IsDeleted == false).Select(s => new GetServiceByIdResponse()
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Price = s.Price,
                    ServiceTime = s.ServiceTime,
                    Images = _mapper.Map<List<ServiceImageResponse>>(_serviceImageRepository.Entities.Where(_ => _.ServiceId == s.Id && _.IsDeleted == false ).ToList())
                }).FirstOrDefault();
            if (service == null) throw new KeyNotFoundException(StaticVariable.NOT_FOUND_MSG);
            return await Result<GetServiceByIdResponse>.SuccessAsync(service);
        }
    }
}

using Application.Dtos.Responses.ServiceImage;
using Application.Exceptions;
using Application.Interfaces;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
        private readonly IUploadService _uploadService;

        public GetServiceByIdQueryHandler(IServiceRepository serviceRepository, IServiceImageRepository serviceImageRepository, IUploadService uploadService)
        {
            _serviceRepository = serviceRepository;
            _serviceImageRepository = serviceImageRepository;
            _uploadService = uploadService;
        }

        public async Task<Result<GetServiceByIdResponse>> Handle(GetServiceByIdQuery request, CancellationToken cancellationToken)
        {
            var service = await (from s in _serviceRepository.Entities
                                 where s.Id == request.Id && !s.IsDeleted
                                 select new GetServiceByIdResponse()
                                 {
                                     Id = s.Id,
                                     Name = s.Name,
                                     Description = s.Description,
                                     Price = s.Price,
                                     ServiceTime = s.ServiceTime,
                                     Images = (from i in _serviceImageRepository.Entities.Where(x => x.ServiceId == s.Id && !x.IsDeleted)
                                               select new ServiceImageResponse()
                                               {
                                                   Id = i.Id,
                                                   ServiceId = i.ServiceId,
                                                   NameFile = i.NameFile,
                                                   NameFileLink = _uploadService.GetFullUrl(i.NameFile)
                                               }).ToList()
                                 }).FirstOrDefaultAsync();
            if (service == null) throw new ApiException(StaticVariable.NOT_FOUND_MSG);
            return await Result<GetServiceByIdResponse>.SuccessAsync(service);
        }
    }
}
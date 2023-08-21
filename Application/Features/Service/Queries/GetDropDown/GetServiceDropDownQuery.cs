using Application.Interfaces.Service;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Application.Features.Service.Queries.GetDropDown
{
    public class GetServiceDropDownQuery : IRequest<Result<List<GetServiceDropDownResponse>>>
    {
    }
    internal class GetServiceDropDownHandler : IRequestHandler<GetServiceDropDownQuery, Result<List<GetServiceDropDownResponse>>>
    {
        private readonly IServiceRepository _serviceRepository;

        public GetServiceDropDownHandler(IServiceRepository serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }
        public Task<Result<List<GetServiceDropDownResponse>>> Handle(GetServiceDropDownQuery request, CancellationToken cancellationToken)
        {
            var query = from s in _serviceRepository.Entities
                        where !s.IsDeleted
                        select new GetServiceDropDownResponse
                        {
                            Id = s.Id,
                            Name = s.Name
                        };

            var totalRecord = query.Count();
            List<GetServiceDropDownResponse> result = query.ToList();

            return Result<List<GetServiceDropDownResponse>>.SuccessAsync(result);
        }
    }
}

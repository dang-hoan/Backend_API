using Application.Features.Cusomter.Queries.GetAll;
using Application.Interfaces.Customer;
using Application.Parameters;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Application.Features.Customer.Queries.GetAll
{
    public class GetAllCustomerQuery : RequestParameter, IRequest<PaginatedResult<GetAllCustomerResponse>>
    {
    }
    internal class GetAllCustomerHandler : IRequestHandler<GetAllCustomerQuery, PaginatedResult<GetAllCustomerResponse>>
    {
        private readonly ICustomerRepository _CustomerRepository;
        public GetAllCustomerHandler(ICustomerRepository CustomerRepository)
        {
            _CustomerRepository = CustomerRepository;
        }
        public async Task<PaginatedResult<GetAllCustomerResponse>> Handle(GetAllCustomerQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var query = _CustomerRepository.Entities.AsEnumerable()
                        .Where(x => !x.IsDeleted 
                                && (string.IsNullOrEmpty(request.Keyword) 
                                || StringHelper.Contains(x.CustomerName, request.Keyword) 
                                || x.PhoneNumber.Contains(request.Keyword)))
                        .AsQueryable()
                        .Select(x => new GetAllCustomerResponse
                        {
                            Id = x.Id,
                            CustomerName = x.CustomerName,
                            PhoneNumber = x.PhoneNumber,
                            Address = x.Address,
                            DateOfBirth = x.DateOfBirth,
                            TotalMoney = x.TotalMoney,
                            CreatedOn = x.CreatedOn,
                            LastModifiedOn = x.LastModifiedOn
                        });
            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllCustomerResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllCustomerResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}

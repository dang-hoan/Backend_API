using System;
using Application.Interfaces.Customer;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Constants;

namespace Application.Features.Customer.Queries.GetById
{
    public class GetCustomerByIdQuery : IRequest<Result<GetCustomerByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<GetCustomerByIdResponse>>
    {
        private readonly ICustomerRepository _CustomerRepository;

        public GetCustomerByIdQueryHandler(ICustomerRepository CustomerRepository)
        {
            _CustomerRepository = CustomerRepository;
        }

        public async Task<Result<GetCustomerByIdResponse>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var Customer = await (from e in _CustomerRepository.Entities
                                  where e.Id == request.Id && !e.IsDeleted
                                  select new GetCustomerByIdResponse()
                                  {
                                      CustomerName = e.CustomerName,
                                      PhoneNumber = e.PhoneNumber,
                                      Address = e.Address,
                                      DateOfBirth = e.DateOfBirth,
                                      TotalMoney = e.TotalMoney
                                  }).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (Customer == null) throw new KeyNotFoundException(StaticVariable.NOT_FOUND_MSG);
            return await Result<GetCustomerByIdResponse>.SuccessAsync(Customer);
        }
    }
}
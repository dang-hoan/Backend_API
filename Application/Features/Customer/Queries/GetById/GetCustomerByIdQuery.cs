using Application.Interfaces.Customer;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Constants;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Features.Customer.Queries.GetById
{
    public class GetCustomerByIdQuery : IRequest<Result<GetCustomerByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, Result<GetCustomerByIdResponse>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<AppUser> _userManager;

        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository, ICurrentUserService currentUserService, UserManager<AppUser> userManager)
        {
            _customerRepository = customerRepository;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        public async Task<Result<GetCustomerByIdResponse>> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            if (_currentUserService.RoleName.Equals(RoleConstants.CustomerRole))
            {
                long userId = _userManager.Users.Where(user => _currentUserService.UserName.Equals(user.UserName)).Select(user => user.UserId).FirstOrDefault();

                if (userId != request.Id)
                    return await Result<GetCustomerByIdResponse>.FailAsync(StaticVariable.NOT_HAVE_ACCESS);
            }

            var Customer = await (from e in _customerRepository.Entities
                                  where e.Id == request.Id && !e.IsDeleted
                                  select new GetCustomerByIdResponse()
                                  {
                                      CustomerName = e.CustomerName,
                                      PhoneNumber = e.PhoneNumber,
                                      Address = e.Address,
                                      DateOfBirth = e.DateOfBirth,
                                      TotalMoney = e.TotalMoney
                                  }).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (Customer == null) return await Result<GetCustomerByIdResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            return await Result<GetCustomerByIdResponse>.SuccessAsync(Customer);
        }
    }
}
using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Account;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using System.ComponentModel.DataAnnotations;
using Domain.Helpers;

namespace Application.Features.Customer.Command.AddCustomer
{
    public class AddCustomerCommand : IRequest<Result<AddCustomerCommand>>
    {
        public long Id { get; set; }
        public string CustomerName { get; set; } = default!;
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [RegularExpression(@"(\+84|84|0)+(3|5|7|8|9|1[2|6|8|9])+([0-9]{7,8})\b", ErrorMessage = StaticVariable.INVALID_PHONE_NUMBER)]
        public string PhoneNumber { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9]+$",  ErrorMessage = StaticVariable.INVALID_USER_NAME )]
        public string? Username { get; set; }

        [RegularExpression(@"^[a-zA-Z0-9!@#$%^&*()-_=+[\]{}|;:',.<>\/?~]{8,}$", ErrorMessage = StaticVariable.INVALID_PASSWORD)]
        public string? Password { get; set; }

        public decimal? TotalMoney { get; set; }
    }

    internal class AddCustomerCommandHandler : IRequestHandler<AddCustomerCommand, Result<AddCustomerCommand>>
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customnerRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IAccountService _accountService;

        public AddCustomerCommandHandler(IMapper mapper, ICustomerRepository customerRepository, IUnitOfWork<long> unitOfWork, IAccountService accountService)
        {
            _mapper = mapper;
            _customnerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _accountService = accountService;
        }

        public async Task<Result<AddCustomerCommand>> Handle(AddCustomerCommand request, CancellationToken cancellationToken)
        {
            bool isPhoneNumberExists = _customnerRepository.Entities.Any(x => x.PhoneNumber.Equals(request.PhoneNumber) && !x.IsDeleted);
            if (isPhoneNumberExists)
            {
                return await Result<AddCustomerCommand>.FailAsync(StaticVariable.PHONE_NUMBER_EXISTS_MSG);
            }
            var addCustomer = _mapper.Map<Domain.Entities.Customer.Customer>(request);
            var errorLimitCharacter = StringHelper.CheckLimitCustomer(addCustomer);
            if (!errorLimitCharacter.Equals(""))
            {
                return await Result<AddCustomerCommand>.FailAsync(errorLimitCharacter);
            }
            if (request.Username != null && request.Username.Length > 50)
            {
                return await Result<AddCustomerCommand>.FailAsync(StaticVariable.LIMIT_USERNAME);
            }
            if (request.Password != null && request.Password.Length > 100)
            {
                return await Result<AddCustomerCommand>.FailAsync(StaticVariable.LIMIT_PASSWORD);
            }
            if(request.Username != null && string.IsNullOrWhiteSpace(request.Password))
            {
                return await Result<AddCustomerCommand>.FailAsync(StaticVariable.REQUIRED_PASSWORD);
            }
            if(string.IsNullOrWhiteSpace(request.Username) && request.Password != null)
            {
                return await Result<AddCustomerCommand>.FailAsync(StaticVariable.REQUIRED_USER_NAME);
            }
            if (!string.IsNullOrWhiteSpace(request.Username) && !string.IsNullOrWhiteSpace(request.Password))
            {
                bool isUsernameExists = await _accountService.IsExistUsername(request.Username);
                if (isUsernameExists)
                {
                    return await Result<AddCustomerCommand>.FailAsync(StaticVariable.IS_EXISTED_USERNAME);
                }
                await _customnerRepository.AddAsync(addCustomer);
                await _unitOfWork.Commit(cancellationToken);
                request.Id = addCustomer.Id;
                var user = new AppUser()
                {
                    FullName = request.CustomerName,
                    UserName = request.Username,
                    EmailConfirmed = false,
                    PhoneNumber = request.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    TypeFlag = TypeFlagEnum.Customer,
                    UserId = request.Id
                };
                bool result = await _accountService.AddAcount(user, request.Password,RoleConstants.CustomerRole);
                if (result == false)
                {
                    return await Result<AddCustomerCommand>.FailAsync(StaticVariable.ERROR_ADD_USER);
                }
                return await Result<AddCustomerCommand>.SuccessAsync(request);
            }
            await _customnerRepository.AddAsync(addCustomer);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = addCustomer.Id;
            return await Result<AddCustomerCommand>.SuccessAsync(request);
        }
    }
}

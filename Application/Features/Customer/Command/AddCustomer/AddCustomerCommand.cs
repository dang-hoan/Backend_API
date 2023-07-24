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

namespace Application.Features.Customer.Command.AddCustomer
{
    public class AddCustomerCommand : IRequest<Result<AddCustomerCommand>>
    {
        public long Id { get; set; }
        public string CustomerName { get; set; } = default!;
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]\d{7,9}$", ErrorMessage = "Phone number must be between 8 and 10 digits.")]
        public string PhoneNumber { get; set; }
        public string? Username { get; set; }
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
            var addCustomer = _mapper.Map<Domain.Entities.Customer.Customer>(request);
            if((request.Username != null && request.Password != null) && (request.Username != "" && request.Password != ""))
            {
                if (request.Password.Length < 8)
                {
                    return await Result<AddCustomerCommand>.FailAsync(StaticVariable.INVALID_PASSWORD);
                }
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

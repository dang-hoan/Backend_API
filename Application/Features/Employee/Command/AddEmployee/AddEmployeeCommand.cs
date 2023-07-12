using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Account;
using AutoMapper;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.Employee;
using Domain.Constants.Enum;

namespace Application.Features.Employee.Command.AddEmployee
{
    public class AddEmployeeCommand : IRequest<Result<AddEmployeeCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool Gender { get; set; }
        public string? Image { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public long WorkShiftId { get; set; }

    }

    internal class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, Result<AddEmployeeCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IAccountService _accountService;


        public AddEmployeeCommandHandler(IMapper mapper, IEmployeeRepository employeeRepository, IUnitOfWork<long> unitOfWork, IAccountService accountService)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _accountService = accountService;
        }

        public async Task<Result<AddEmployeeCommand>> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
            bool isUsernameExists = await _accountService.IsExistUsername(request.Username);
            if (isUsernameExists)
            {
                return await Result<AddEmployeeCommand>.FailAsync("This username already exists in the database.");
            }
            var existEmail = _employeeRepository.Entities.FirstOrDefault(_ => _.Email == request.Email && _.IsDeleted == false);
            if (existEmail != null)
            {
                return await Result<AddEmployeeCommand>.FailAsync("This email already exists in the database.");
            }
            if (request.Password.Length < 8)
            {
                return await Result<AddEmployeeCommand>.FailAsync("Password must be at least 8 characters");
            }
            var addEmployee = _mapper.Map<Domain.Entities.Employee.Employee>(request);
            await _employeeRepository.AddAsync(addEmployee);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = addEmployee.Id;
            var user = new AppUser()
            {
                FullName = request.Name,
                Email = request.Email,
                UserName = request.Username,
                EmailConfirmed = true,
                PhoneNumber = request.PhoneNumber,
                PhoneNumberConfirmed = true,
                CreatedOn = DateTime.Now,
                IsActive = true,
                TypeFlag = TypeFlagEnum.Employee,
                UserId = request.Id,
            };
            bool result = await _accountService.AddAcount(user, request.Password);
            if (result == false)
            {
                return await Result<AddEmployeeCommand>.FailAsync("There was an error during the account creation process.");
            }
            return await Result<AddEmployeeCommand>.SuccessAsync(request);
        }
    }
}

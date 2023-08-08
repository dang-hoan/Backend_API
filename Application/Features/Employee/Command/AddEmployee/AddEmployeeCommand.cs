using Application.Interfaces.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Account;
using Application.Interfaces.WorkShift;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Features.Employee.Command.AddEmployee
{
    public class AddEmployeeCommand : IRequest<Result<AddEmployeeCommand>>
    {
        public long? Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = default!;

        public string PhoneNumber { get; set; }
        public bool? Gender { get; set; }
        public string? Image { get; set; }
        public string Password { get; set; } = default!;
        public string Username { get; set; } = default!;
        public long WorkShiftId { get; set; }
    }

    internal class AddEmployeeCommandHandler : IRequestHandler<AddEmployeeCommand, Result<AddEmployeeCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IAccountService _accountService;
        private readonly IWorkShiftRepository _workshiftRepository;

        public AddEmployeeCommandHandler(IMapper mapper, IEmployeeRepository employeeRepository, IUnitOfWork<long> unitOfWork, IAccountService accountService, IWorkShiftRepository workshiftRepository)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _accountService = accountService;
            _workshiftRepository = workshiftRepository;
        }

        public async Task<Result<AddEmployeeCommand>> Handle(AddEmployeeCommand request, CancellationToken cancellationToken)
        {
            request.Id = null;
            bool isUsernameExists = await _accountService.IsExistUsername(request.Username);
            if (isUsernameExists)
            {
                return await Result<AddEmployeeCommand>.FailAsync(StaticVariable.USERNAME_EXISTS_MSG);
            }
            var existEmail = _employeeRepository.Entities.FirstOrDefault(x => x.Email == request.Email && !x.IsDeleted);
            if (existEmail != null)
            {
                return await Result<AddEmployeeCommand>.FailAsync(StaticVariable.EMAIL_EXISTS_MSG);
            }
            if (request.Password.Length < 8)
            {
                return await Result<AddEmployeeCommand>.FailAsync(StaticVariable.INVALID_PASSWORD);
            }
            if (request.PhoneNumber.Length < 8 || request.PhoneNumber.Length > 10)
            {
                return await Result<AddEmployeeCommand>.FailAsync(StaticVariable.PHONE_ERROR_MSG);
            }
            var isExistedWorkshift = await _workshiftRepository.FindAsync(x => !x.IsDeleted && x.Id == request.WorkShiftId);
            if (isExistedWorkshift == null) return await Result<AddEmployeeCommand>.FailAsync(StaticVariable.NOT_FOUND_WORK_SHIFT);
            var addEmployee = _mapper.Map<Domain.Entities.Employee.Employee>(request);
            await _employeeRepository.AddAsync(addEmployee);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = addEmployee.Id;
            var user = _mapper.Map<AppUser>(request);
            bool result = await _accountService.AddAcount(user, request.Password, RoleConstants.EmployeeRole);
            if (!result)
            {
                return await Result<AddEmployeeCommand>.FailAsync(StaticVariable.ERROR_ADD_USER);
            }
            return await Result<AddEmployeeCommand>.SuccessAsync(request);
        }
    }
}
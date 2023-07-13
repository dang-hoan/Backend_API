using Application.Interfaces.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Identity;
using AutoMapper;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;
using System.ComponentModel.DataAnnotations;


namespace Application.Features.Employee.Command.EditEmployee
{
    public class EditEmployeeCommand : IRequest<Result<EditEmployeeCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool Gender { get; set; }
        public string? Image { get; set; }
        public long WorkShiftId { get; set; }
    }

    internal class AddEmployeeCommandHandler : IRequestHandler<EditEmployeeCommand, Result<EditEmployeeCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IUserService _userService;


        public AddEmployeeCommandHandler(IMapper mapper, IEmployeeRepository employeeRepository, IUnitOfWork<long> unitOfWork, IUserService userService)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _userService = userService;
        }

        public async Task<Result<EditEmployeeCommand>> Handle(EditEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                return await Result<EditEmployeeCommand>.FailAsync("This employee doesn't exist in the database");
            }
            var editEmployee = await _employeeRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted) ?? throw new KeyNotFoundException("This employee doesn't exist in database");
            if(request.Email != editEmployee.Email)
            {
                var existEmail = _employeeRepository.Entities.FirstOrDefault(_ => _.Email == request.Email && _.IsDeleted == false);
                if (existEmail != null)
                {
                    return await Result<EditEmployeeCommand>.FailAsync("New email already exists in the database.");
                }
            }
            _mapper.Map(request, editEmployee);
            await _employeeRepository.UpdateAsync(editEmployee);
            await _unitOfWork.Commit(cancellationToken);
            await _userService.EditUser(new Dtos.Requests.Identity.EditUserRequest
            {
                Id = request.Id,
                TypeFlag = TypeFlagEnum.Employee,
                FullName = request.Name,
                Email = request.Email,
                Phone = request.PhoneNumber
            });
            return await Result<EditEmployeeCommand>.SuccessAsync(request);
        }
    }
}

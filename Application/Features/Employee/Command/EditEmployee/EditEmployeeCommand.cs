using Application.Interfaces;
using Application.Interfaces.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Identity;
using Application.Interfaces.WorkShift;
using AutoMapper;
using Domain.Constants;
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
        public string Email { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;
        public bool? Gender { get; set; }
        public string? Image { get; set; }
        public long WorkShiftId { get; set; }
    }

    internal class EditEmployeeCommandHandler : IRequestHandler<EditEmployeeCommand, Result<EditEmployeeCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IUserService _userService;
        private readonly IWorkShiftRepository _workshiftRepository;
        private readonly IUploadService _uploadService;

        public EditEmployeeCommandHandler(IMapper mapper, IEmployeeRepository employeeRepository, IUnitOfWork<long> unitOfWork, 
            IUserService userService, IWorkShiftRepository workshiftRepository, IUploadService uploadService)
        {
            _mapper = mapper;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _workshiftRepository = workshiftRepository;
            _uploadService = uploadService;
        }

        public async Task<Result<EditEmployeeCommand>> Handle(EditEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                return await Result<EditEmployeeCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }
            var editEmployee = await _employeeRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (editEmployee == null) return await Result<EditEmployeeCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            if (request.Email != editEmployee.Email)
            {
                var existEmail = _employeeRepository.Entities.FirstOrDefault(x => x.Email == request.Email && !x.IsDeleted);
                if (existEmail != null)
                {
                    return await Result<EditEmployeeCommand>.FailAsync(StaticVariable.EMAIL_EXISTS_MSG);
                }
            }
            if (request.PhoneNumber.Length < 8 || request.PhoneNumber.Length > 10)
            {
                return await Result<EditEmployeeCommand>.FailAsync(StaticVariable.PHONE_ERROR_MSG);
            }
            var isExistedWorkshift = await _workshiftRepository.FindAsync(x => !x.IsDeleted && x.Id == request.WorkShiftId);
            if (isExistedWorkshift == null) return await Result<EditEmployeeCommand>.FailAsync(StaticVariable.NOT_FOUND_WORK_SHIFT);

            if(editEmployee.Image != null)
            {
                await _uploadService.DeleteAsync(editEmployee.Image);
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
                Phone = request.PhoneNumber,
                ImageFile = request.Image,
            });
            return await Result<EditEmployeeCommand>.SuccessAsync(request);
        }
    }
}
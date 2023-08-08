using Application.Interfaces;
using Application.Interfaces.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Identity;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Employee.Command.DeleteEmployee
{
    public class DeleteEmployeeCommand : IRequest<Result<long>>
    {
        public long Id { get; set; }
    }

    internal class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Result<long>>
    {
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUserService _userService;
        private readonly IUploadService _uploadService;

        public DeleteEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork<long> unitOfWork, IUserService userService, IUploadService uploadService)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _userService = userService;
            _uploadService = uploadService;
        }

        public async Task<Result<long>> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var deleteEmployee = await _employeeRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (deleteEmployee == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            await _employeeRepository.DeleteAsync(deleteEmployee);
            if (!string.IsNullOrWhiteSpace(deleteEmployee.Image))
            {
                await _uploadService.DeleteAsync(deleteEmployee.Image);
            }
            await _userService.DeleteUser(new Dtos.Requests.Identity.DeleteUserRequest
            {
                Id = request.Id,
                TypeFlag = TypeFlagEnum.Employee
            });
            await _unitOfWork.Commit(cancellationToken);
            return await Result<long>.SuccessAsync(request.Id, $"Delete employee by id {request.Id} successfully!");
        }
    }
}
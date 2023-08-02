using Application.Interfaces.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Identity;
using Application.Interfaces.WorkShift;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;

namespace Application.Features.Employee.Command.EditWorkShiftEmployee
{
    public class EditWorkShiftEmployeeCommand : IRequest<Result<EditWorkShiftEmployeeCommand>>
    {
        public List<long> ListId { get; set; }
        public long WorkShiftId { get; set; }
    }

    internal class EditWorkShiftEmployeeCommandHandler : IRequestHandler<EditWorkShiftEmployeeCommand, Result<EditWorkShiftEmployeeCommand>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWorkShiftRepository _workShiftRepository;
        private readonly IUnitOfWork<long> _unitOfWork;

        public EditWorkShiftEmployeeCommandHandler(IEmployeeRepository employeeRepository, IUnitOfWork<long> unitOfWork, IUserService userService, IWorkShiftRepository workShiftRepository)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _workShiftRepository = workShiftRepository;
        }

        public async Task<Result<EditWorkShiftEmployeeCommand>> Handle(EditWorkShiftEmployeeCommand request, CancellationToken cancellationToken)
        {
            if (request.WorkShiftId == 0)
            {
                return await Result<EditWorkShiftEmployeeCommand>.FailAsync(StaticVariable.NOT_FOUND_WORK_SHIFT);
            }
            var isExistedWorkShift = await _workShiftRepository.FindAsync(_ => _.Id == request.WorkShiftId && _.IsDeleted == false);
            if (isExistedWorkShift == null) return await Result<EditWorkShiftEmployeeCommand>.FailAsync(StaticVariable.NOT_FOUND_WORK_SHIFT);

            // check request.ListId exist in db
            List<long> listExistEmployeeId = _employeeRepository.Entities.Where(_ => !_.IsDeleted).Select(_ => _.Id).ToList();
            if (request.ListId.Except(listExistEmployeeId).ToList().Any()) return await Result<EditWorkShiftEmployeeCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);

            List<Domain.Entities.Employee.Employee> Employees = _employeeRepository.Entities.Where(_ => request.ListId.Contains(_.Id)).ToList();
            Employees.ForEach(_ => _.WorkShiftId = request.WorkShiftId);
            await _employeeRepository.UpdateRangeAsync(Employees);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<EditWorkShiftEmployeeCommand>.SuccessAsync(StaticVariable.SUCCESS);
        }
    }
}
using Application.Interfaces.Repositories;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.WorkShift;
using Domain.Constants;
using Application.Interfaces.Employee;

namespace Application.Features.WorkShift.Command.DeleteWorkShift
{
    public class DeleteWorkShiftCommand : IRequest<Result<long>>
    {
        public long Id { get; set; }
    }

    internal class DeleteWorkShiftCommandHandler : IRequestHandler<DeleteWorkShiftCommand, Result<long>>
    {
        private readonly IWorkShiftRepository _workShiftRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork<long> _unitOfWork;


        public DeleteWorkShiftCommandHandler(IWorkShiftRepository workShiftRepository, IEmployeeRepository employeeRepository, IUnitOfWork<long> unitOfWork)
        {
            _workShiftRepository = workShiftRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<long>> Handle(DeleteWorkShiftCommand request, CancellationToken cancellationToken)
        {
            var workShift = await _workShiftRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (workShift == null) return await Result<long>.FailAsync(StaticVariable.NOT_FOUND_MSG);

            var workShiftIsAssigned = await _employeeRepository.GetByCondition(x => x.WorkShiftId == request.Id && !x.IsDeleted);
            if (workShiftIsAssigned.Any())
            {
                return await Result<long>.FailAsync(StaticVariable.WORK_SHIFT_ASSIGNED);
            }
            
            await _workShiftRepository.DeleteAsync(workShift);
            await _unitOfWork.Commit(cancellationToken);

            return await Result<long>.SuccessAsync(request.Id, $"Delete work shift by id {request.Id} successfully!");
        }
    }
}

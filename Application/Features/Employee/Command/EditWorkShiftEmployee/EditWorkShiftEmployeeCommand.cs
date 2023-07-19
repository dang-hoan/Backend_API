using Application.Interfaces.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Identity;
using AutoMapper;
using Domain.Constants;
using Domain.Wrappers;
using MediatR;
using Application.Interfaces.WorkShift;

namespace Application.Features.Employee.Command.EditWorkShiftEmployee
{
    public class EditWorkShiftEmployeeCommand : IRequest<Result<EditWorkShiftEmployeeCommand>>
    {
        public List<long> ListId { get; set; }
        public long WorkShiftId { get; set; }
    }

    internal class EditWorkShiftEmployeeCommandHandler : IRequestHandler<EditWorkShiftEmployeeCommand, Result<EditWorkShiftEmployeeCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWorkShiftRepository _workShiftRepository;
        private readonly IUnitOfWork<long> _unitOfWork;


        public EditWorkShiftEmployeeCommandHandler(IMapper mapper, IEmployeeRepository employeeRepository, IUnitOfWork<long> unitOfWork, IUserService userService, IWorkShiftRepository workShiftRepository)
        {
            _mapper = mapper;
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
            var isExistedWorkShift = await _workShiftRepository.FindAsync(_ => _.Id == request.WorkShiftId && _.IsDeleted == false) ?? throw new KeyNotFoundException(StaticVariable.NOT_FOUND_WORK_SHIFT);
            foreach(long i in request.ListId)
            {
                bool isExistedIdEmployee = await CheckExistedIdEmployee(i);
                if (!isExistedIdEmployee)
                {
                    throw new KeyNotFoundException($"IdEmployee = {i} does not exist in the database");
                }
            }
            List<Domain.Entities.Employee.Employee> Employees = _employeeRepository.Entities.Where(_ => request.ListId.Contains(_.Id)).ToList();
            Employees.ForEach(_ => _.WorkShiftId = request.WorkShiftId);
            await _employeeRepository.UpdateRangeAsync(Employees);
            await _unitOfWork.Commit(cancellationToken);
            return await Result<EditWorkShiftEmployeeCommand>.SuccessAsync(StaticVariable.SUCCESS);
        }
        public async Task<bool> CheckExistedIdEmployee(long id)
        {
            var isExistedId = await _employeeRepository.FindAsync(_ => _.Id == id && _.IsDeleted == false);
            return isExistedId != null;
        }
    }
}

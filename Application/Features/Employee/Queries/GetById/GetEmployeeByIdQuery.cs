using System;
using Application.Interfaces.Employee;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Constants;

namespace Application.Features.Employee.Queries.GetById
{
    public class GetEmployeeByIdQuery : IRequest<Result<GetEmployeeByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Result<GetEmployeeByIdResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<Result<GetEmployeeByIdResponse>> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var employee = await (from e in _employeeRepository.Entities
                                  where e.Id == request.Id && !e.IsDeleted
                                  select new GetEmployeeByIdResponse()
                                  {
                                      Id = e.Id,
                                      Name = e.Name,
                                      Email = e.Email,
                                      Gender = e.Gender,
                                      Birthday = e.Birthday,
                                      PhoneNumber = e.PhoneNumber,
                                      Address = e.Address,
                                      Image = e.Image,
                                      WorkShiftId = e.WorkShiftId
                                  }).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (employee == null) throw new KeyNotFoundException(StaticVariable.NOT_FOUND_MSG);
            return await Result<GetEmployeeByIdResponse>.SuccessAsync(employee);
        }
    }
}
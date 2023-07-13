using Application.Interfaces.Employee;
using Application.Parameters;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employee.Queries.GetAll
{
    public class GetAllEmployeeQuery : GetAllEmployeeParameter, IRequest<PaginatedResult<GetAllEmployeeResponse>>
    {
    }
    internal class GetAllEmployeeHandler : IRequestHandler<GetAllEmployeeQuery, PaginatedResult<GetAllEmployeeResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        public GetAllEmployeeHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public async Task<PaginatedResult<GetAllEmployeeResponse>> Handle(GetAllEmployeeQuery request, CancellationToken cancellationToken)
        {
            var query = _employeeRepository.Entities.Where(x => !x.IsDeleted
                                                                && (string.IsNullOrEmpty(request.Keyword) || x.Name.Contains(request.Keyword) || x.Id.ToString().Contains(request.Keyword))
                                                                && (!request.Gender.HasValue || x.Gender == request.Gender)
                                                                && (!request.WorkShiftId.HasValue || x.WorkShiftId == request.WorkShiftId))
                                           .Select(x => new GetAllEmployeeResponse
                                           {
                                               Id = x.Id,
                                               Name = x.Name,
                                               Gender = x.Gender,
                                               PhoneNumber = x.PhoneNumber,
                                               CreatedOn = x.CreatedOn,
                                               LastModifiedOn = x.LastModifiedOn,
                                               WorkShiftId = x.WorkShiftId,
                                           });
            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllEmployeeResponse> result;

            //Pagination
            if (!request.IsExport)
                result = await data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            else
                result = await data.ToListAsync(cancellationToken);
            return PaginatedResult<GetAllEmployeeResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}

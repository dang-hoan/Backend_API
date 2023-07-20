using Application.Interfaces.Employee;
using Application.Interfaces.WorkShift;
using Application.Parameters;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using Domain.Wrappers;
using MediatR;


namespace Application.Features.WorkShift.Queries.GetAll
{
    public class GetAllWorkShiftQuery : RequestParameter, IRequest<PaginatedResult<GetAllWorkShiftResponse>>
    {
    }
    internal class GetAllWorkShiftHandler : IRequestHandler<GetAllWorkShiftQuery, PaginatedResult<GetAllWorkShiftResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IWorkShiftRepository _workShiftRepository;
        public GetAllWorkShiftHandler(IEmployeeRepository employeeRepository, IWorkShiftRepository workShiftRepository)
        {
            _employeeRepository = employeeRepository;
            _workShiftRepository = workShiftRepository;
        }
        public async Task<PaginatedResult<GetAllWorkShiftResponse>> Handle(GetAllWorkShiftQuery request, CancellationToken cancellationToken)
        {
            var query = _workShiftRepository.Entities.Where(x => !x.IsDeleted
                                                                && (string.IsNullOrEmpty(request.Keyword) || x.Name.Contains(request.Keyword) || x.Id.ToString().Contains(request.Keyword)))

                                        .Select(x => new GetAllWorkShiftResponse
                                        {
                                            Id = x.Id,
                                            Name = x.Name,
                                            IsDefault = x.IsDefault,
                                            FromTime = x.WorkingFromTime.ToString(@"hh\:mm"),
                                            ToTime = x.WorkingToTime.ToString(@"hh\:mm"),
                                            TimeWord = (x.WorkingToTime - x.WorkingFromTime).TotalHours,
                                            NumberEmployee = _employeeRepository.Entities.Where(_ => _.WorkShiftId == x.Id && _.IsDeleted == false).Count(),
                                            Description = x.Description,
                                            //WorkDays = x.WorkDays.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList(),
                                            CreatedOn = x.CreatedOn,
                                            LastModifiedOn = x.LastModifiedOn
                                        });

            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllWorkShiftResponse> result;

            //Pagination
            if (!request.IsExport)
                result = await data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToListAsync(cancellationToken);
            else
                result = await data.ToListAsync(cancellationToken);
            return PaginatedResult<GetAllWorkShiftResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public List<int> convertToInt(string[] list)
        {
            return list.ToList().ConvertAll(int.Parse);
        }
    }
}

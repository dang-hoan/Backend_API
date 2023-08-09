using Application.Interfaces.Employee;
using Application.Interfaces.WorkShift;
using Application.Parameters;
using System.Linq.Dynamic.Core;
using Domain.Wrappers;
using MediatR;
using Domain.Helpers;

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
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var query = _workShiftRepository.Entities.AsEnumerable()
                        .Where(x => !x.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                                                 || StringHelper.Contains(x.Name, request.Keyword) 
                                                 || x.Id.ToString().Contains(request.Keyword)))
                        .AsQueryable()
                        .Select(x => new GetAllWorkShiftResponse
                        {
                            Id = x.Id,
                            Name = x.Name,
                            IsDefault = x.IsDefault,
                            FromTime = x.WorkingFromTime.ToString(@"hh\:mm"),
                            ToTime = x.WorkingToTime.ToString(@"hh\:mm"),
                            TimeWork = (x.WorkingToTime - x.WorkingFromTime).TotalHours,
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
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllWorkShiftResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public List<int> convertToInt(string[] list)
        {
            return list.ToList().ConvertAll(int.Parse);
        }
    }
}

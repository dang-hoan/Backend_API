using Application.Interfaces;
using Application.Interfaces.Employee;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using System.Linq.Dynamic.Core;

namespace Application.Features.Employee.Queries.GetAll
{
    public class GetAllEmployeeQuery : GetAllEmployeeParameter, IRequest<PaginatedResult<GetAllEmployeeResponse>>
    {
    }

    internal class GetAllEmployeeHandler : IRequestHandler<GetAllEmployeeQuery, PaginatedResult<GetAllEmployeeResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUploadService _uploadService;

        public GetAllEmployeeHandler(IEmployeeRepository employeeRepository, IUploadService uploadService)
        {
            _employeeRepository = employeeRepository;
            _uploadService = uploadService;
        }

        public async Task<PaginatedResult<GetAllEmployeeResponse>> Handle(GetAllEmployeeQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var query = _employeeRepository.Entities.AsEnumerable()
                        .Where(x => !x.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                                                || StringHelper.Contains(x.Name, request.Keyword) || x.Id.ToString().Contains(request.Keyword))
                                                && (!request.Gender.HasValue || x.Gender == request.Gender)
                                                && (!request.WorkShiftId.HasValue || x.WorkShiftId == request.WorkShiftId))
                        .AsQueryable()
                        .Select(x => new GetAllEmployeeResponse
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Gender = x.Gender,
                            PhoneNumber = x.PhoneNumber,
                            CreatedOn = x.CreatedOn,
                            LastModifiedOn = x.LastModifiedOn,
                            WorkShiftId = x.WorkShiftId,
                            Email = x.Email,
                            ImageFile = x.Image,
                            ImageLink = _uploadService.GetFullUrl(x.Image)
                        });
            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllEmployeeResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllEmployeeResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}
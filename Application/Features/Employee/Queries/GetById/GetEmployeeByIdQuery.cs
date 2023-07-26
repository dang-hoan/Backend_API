using Application.Interfaces.Employee;
using Domain.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Application.Interfaces;

namespace Application.Features.Employee.Queries.GetById
{
    public class GetEmployeeByIdQuery : IRequest<Result<GetEmployeeByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, Result<GetEmployeeByIdResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUploadService _uploadService;

        public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository, UserManager<AppUser> userManager, IHttpContextAccessor httpContextAccessor, IUploadService uploadService)
        {
            _employeeRepository = employeeRepository;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _uploadService = uploadService;
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
                                      WorkShiftId = e.WorkShiftId,
                                      UserName = _userManager.Users.Where(e => e.UserId == request.Id && !e.IsDeleted).Select(e => e.UserName).FirstOrDefault(),
                                      Password = _userManager.Users.Where(e => e.UserId == request.Id && !e.IsDeleted).Select(e => e.PasswordHash).FirstOrDefault()
                                  }).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (employee == null) throw new KeyNotFoundException(StaticVariable.NOT_FOUND_MSG);

            if (employee.Image != null)
                employee.Image = _uploadService.GetFileLink(employee.Image, _httpContextAccessor);

            return await Result<GetEmployeeByIdResponse>.SuccessAsync(employee);
        }
    }
}
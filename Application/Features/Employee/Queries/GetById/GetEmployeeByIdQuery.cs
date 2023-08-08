using Application.Interfaces;
using Application.Interfaces.Employee;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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
        private readonly IUploadService _uploadService;

        public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository, UserManager<AppUser> userManager, IUploadService uploadService)
        {
            _employeeRepository = employeeRepository;
            _userManager = userManager;
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
                                      ImageLink = _uploadService.GetFullUrl(e.Image),
                                      UserName = _userManager.Users.Where(e => e.UserId == request.Id && !e.IsDeleted).Select(e => e.UserName).FirstOrDefault()
                                  }).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (employee == null) return await Result<GetEmployeeByIdResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);

            return await Result<GetEmployeeByIdResponse>.SuccessAsync(employee);
        }
    }
}
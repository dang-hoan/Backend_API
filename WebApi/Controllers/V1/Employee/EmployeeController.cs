using Application.Features.Employee.Querries.GetAll;
using Domain.Wrappers;
using Application.Features.Employee.Command.AddEmployee;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Employee
{
    [Route("api/v{version:apiVersion}/employee")]
    [ApiController]
    public class EmployeeController : BaseApiController<EmployeeController>
    {
        /// <summary>
        /// Get all Employee pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        //[Authorize("Superadmin")]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllEmployeeResponse>>> GetAllEmployee([FromQuery] GetAllEmployeeParameter parameter)
        {
            return Ok(await Mediator.Send(new GetAllEmployeeQuery() {
                IsExport = parameter.IsExport,
                Keyword = parameter.Keyword,
                OrderBy = parameter.OrderBy,
                PageNumber = parameter.PageNumber,
                PageSize = parameter.PageSize,
                Gender = parameter.Gender,
                MaxBirthDay = parameter.MaxBirthDay,
                MinBirthDay = parameter.MinBirthDay
            }));
        }
        /// <summary>
        /// Add/Edit Employee
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployeeCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
    }
}

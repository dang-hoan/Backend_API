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

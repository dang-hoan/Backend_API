using Application.Features.Employee.Queries.GetById;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Application.Features.Employee.Command.AddEmployee;
using Application.Features.Employee.Command.DeleteEmployee;
using Application.Features.Employee.Queries.GetAll;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Employee.Command.EditEmployee;

namespace WebApi.Controllers.V1.Employee
{
    [ApiController]

    [Route("api/v{version:apiVersion}/employee")]
    public class EmployeeController : BaseApiController<EmployeeController>
    {
        /// <summary>
        /// Get Employee detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<GetEmployeeByIdResponse>>> GetEmployeeById(short id)
        {
            return Ok(await Mediator.Send(new GetEmployeeByIdQuery()
            {
                Id = id
            }));
        }


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
                WorkShiftId = parameter.WorkShiftId,
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
            var result = await Mediator.Send(command);
            if (result.Succeeded == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        
        /// <summary>
        /// Delete Employee by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteEmployee(short id)
        {
            return Ok(await Mediator.Send(new DeleteEmployeeCommand
            {
                Id = id
            }));
        }

        /// <summary>
        /// Add/Edit Employee
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut]
        public async Task<IActionResult> EditEmployee(EditEmployeeCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Succeeded == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
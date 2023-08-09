using Application.Features.WorkShift.Command.DeleteWorkShift;
using Application.Features.WorkShift.Command.AddWorkShift;
using Application.Features.WorkShift.Queries.GetAll;
using Application.Parameters;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Application.Features.WorkShift.Command.EditWorkShift;
using Application.Features.WorkShift.Queries.GetById;
using Microsoft.AspNetCore.Authorization;
using Domain.Constants;

namespace WebApi.Controllers.V1.WorkShift
{
    [ApiController]
    [Route("api/v{version:apiVersion}/workshift")]
    public class WorkShiftController : BaseApiController<WorkShiftController>
    {
        /// <summary>
        /// Get all Workshift pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllWorkShiftResponse>>> GetAllWorkShift([FromQuery] RequestParameter parameter)
        {
            return Ok(await Mediator.Send(new GetAllWorkShiftQuery()
            {
                IsExport = parameter.IsExport,
                Keyword = parameter.Keyword,
                OrderBy = parameter.OrderBy,
                PageNumber = parameter.PageNumber,
                PageSize = parameter.PageSize,
            }));
        }

        /// <summary>
        /// Add WorkShift
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpPost]
        public async Task<IActionResult> AddWorkShift(AddWorkShiftCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Edit WorkShift
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpPut]
        public async Task<IActionResult> EditWorkShift(EditWorkShiftCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete work shift by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpDelete]
        public async Task<IActionResult> DeleteWorkShift(long id)
        {
            var result = await Mediator.Send(new DeleteWorkShiftCommand
            {
                Id = id
            });
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
        /// <summary>
        /// Get Workshfit detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<GetWorkshiftByIdResponse>>> GetWorkshiftById(long id)
        {
            return Ok(await Mediator.Send(new GetWorkshiftByIdQuery()
            {
                Id = id
            }));
        }
    }
}

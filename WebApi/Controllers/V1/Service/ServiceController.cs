using Application.Features.Service.Queries.GetAll;
using Application.Features.Service.Command.AddService;
using Application.Features.Service.Queries.GetById;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Service.Command.EditService;
using Microsoft.AspNetCore.Authorization;
using Application.Features.Service.Command.DeleteService;
using Domain.Constants;
using Application.Features.Service.Queries.GetDropDown;

namespace WebApi.Controllers.V1.Service
{
    [ApiController]
    [Route("api/v{version:apiVersion}/service")]
    public class ServiceController : BaseApiController<ServiceController>
    {
        /// <summary>
        /// Get all Serivce pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllServiceResponse>>> GetAllService([FromQuery] GetAllServiceParameter parameter)
        {
            return Ok(await Mediator.Send(new GetAllServiceQuery()
            {
                IsExport = parameter.IsExport,
                Keyword = parameter.Keyword,
                OrderBy = parameter.OrderBy,
                PageNumber = parameter.PageNumber,
                PageSize = parameter.PageSize,
                Time = parameter.Time,
                Review = parameter.Review,
            }));
        }

        /// <summary>
        /// Get Serivce DropDown, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [HttpGet("dropdown")]
        public async Task<ActionResult<PaginatedResult<GetServiceDropDownResponse>>> GetServiceDropDown()
        {
            return Ok(await Mediator.Send(new GetServiceDropDownQuery()));
        }

        /// <summary>
        /// Add/Edit Service
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpPost]
        public async Task<IActionResult> AddService(AddServiceCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpPut]
        public async Task<IActionResult> EditService(EditServiceCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Delete Service by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpDelete]
        public async Task<IActionResult> DeleteService(short id)
        {
            var result = await Mediator.Send(new DeleteServiceCommand
            {
                Id = id
            });
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
        /// <summary>
        /// Get Service detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<GetServiceByIdResponse>>> GetServiceById(short id)
        {
            return Ok(await Mediator.Send(new GetServiceByIdQuery()
            {
                Id = id
            }));
        }
    }
}

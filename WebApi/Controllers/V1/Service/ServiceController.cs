using Application.Features.Service.Queries.GetAll;
using Application.Features.Service.Command.AddService;
using Application.Features.Service.Queries.GetById;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Service.Command.EditService;
using Microsoft.AspNetCore.Authorization;
using Application.Features.Service.Command.DeleteService;

namespace WebApi.Controllers.V1.Service
{
    [ApiController]
    [Route("api/v{version:apiVersion}/service")]
    public class ServiceController : BaseApiController<ServiceController>
    {
        /// Get all Serivce pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        // [Authorize(Roles = "SUPERADMIN")]
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
        /// Add/Edit Service
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddService([FromForm] AddServiceCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        //[Authorize]
        [HttpPut]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditService([FromForm] EditServiceCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        /// <summary>
        /// Delete Service by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteService(short id)
        {
            return Ok(await Mediator.Send(new DeleteServiceCommand
            {
                Id = id
            }));
        }
        /// <summary>
        /// Get Service detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

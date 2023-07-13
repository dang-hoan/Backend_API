using Application.Features.Service.Command.AddService;
using Application.Features.Service.Queries.GetById;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;


namespace WebApi.Controllers.V1.Service
{
    [ApiController]
    [Route("api/v{version:apiVersion}/service")]
    public class ServiceController : BaseApiController<ServiceController>
    {       
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

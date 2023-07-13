using Application.Features.Service.Command.AddService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;


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
    }
}

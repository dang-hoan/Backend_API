using Application.Features.Reply.Command.AddReplyAtFeeback;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Feeback
{
    [ApiController]
    [Route("api/v{version:apiVersion}/feedback")]
    public class FeedbackController : BaseApiController<FeedbackController>
    {
        /// <summary>
        /// Add reply at feeback
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("reply")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddEmployee([FromForm] AddReplyAtFeedbackCommand command)
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

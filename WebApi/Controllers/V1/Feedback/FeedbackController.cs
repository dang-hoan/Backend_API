using Application.Features.Reply.Command.AddReplyAtFeeback;
using Application.Features.Reply.Command.EditReply;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Feeback
{
    [ApiController]
    [Route("api/v{version:apiVersion}/feedback")]
    public class FeedbackController : BaseApiController<FeedbackController>
    {
        /// <summary>
        /// Add reply at feedback
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("reply")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddReplyAtFeedback([FromForm] AddReplyAtFeedbackCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Succeeded == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Edit reply
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut("reply")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> EditReply([FromForm] EditReplyCommand command)
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

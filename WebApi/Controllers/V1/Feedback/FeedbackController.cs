using Application.Features.Reply.Command.AddReplyAtFeeback;
using Application.Features.Feedback.Queries.GetAll;
using Microsoft.AspNetCore.Mvc;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Feeback
{
    [ApiController]
    [Route("api/v{version:apiVersion}/feedback")]
    public class FeedbackController : BaseApiController<FeedbackController>
    {
        /// Get all Feedback pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllFeedbackResponse>>> GetAllFeedback([FromQuery] GetAllFeedbackParameter parameter)
        {
            return Ok(await Mediator.Send(new GetAllFeedbackQuery()
            {
                IsExport = parameter.IsExport,
                Keyword = parameter.Keyword,
                OrderBy = parameter.OrderBy,
                PageNumber = parameter.PageNumber,
                PageSize = parameter.PageSize,
                ServiceName = parameter.ServiceName,
                Rating = parameter.Rating
            }));
        }

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

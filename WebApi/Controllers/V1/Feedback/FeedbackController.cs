using Application.Features.Booking.Queries.GetCustomerBookingHistory;
using Application.Features.Customer.Command.DeleteCustomer;
using Application.Features.Feedback.Command.DeleteFeedback;
using Application.Features.Feedback.Queries.GetById;
using Application.Features.Reply.Command.AddReplyAtFeeback;
using Application.Features.Feedback.Queries.GetAll;
using Application.Features.Reply.Command.EditReply;
using Microsoft.AspNetCore.Mvc;
using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Feedback.Queries.GetHistoryFeedback;
using Application.Features.Client.Command.AddFeedback;

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
        /// <summary>
        /// Delete feedback
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteFeedback(long Id)
        {
            return Ok(await Mediator.Send(new DeleteFeedbackCommand
            {
                Id = Id
            }));
        }
        /// <summary>
        /// Get feedback by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackById(long id)
        {
            return Ok(await Mediator.Send(new GetFeedbackByIdQuery
            {
                Id = id
            }));
        }
        /// <summary>
        /// Get feedback history 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("customer/booking/{id}")]
        public async Task<IActionResult> GetFeedbackHistory(long id)
        {
            return Ok(await Mediator.Send(new GetFeedbackHistoryQuery
            {
                BookingId = id
            }));
        }

        /// <summary>
        /// Add feedback from customer
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost("mybooking")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(50 * 1024 * 1024)] //50MB
        public async Task<IActionResult> AddFeedback([FromForm] AddFeedbackCommand command)
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

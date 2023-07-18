using Application.Features.Booking.Command.AddBooking;
using Application.Features.Booking.Command.DeleteBooking;
using Application.Features.Booking.Queries.GetById;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Booking
{
    [ApiController]
    [Route("api/v{version:apiVersion}/booking")]
    public class BookingController : BaseApiController<BookingController>
    {
        /// <summary>
        /// Add Booking
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> AddBooking(AddBookingCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Succeeded == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Get Booking Detail
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{Id}")]
        public async Task<IActionResult> BookingDetail(long Id)
        {
            var result = await Mediator.Send(new GetBookingByIdQuery
            {
                Id = Id
            });
            return Ok(result);
        }

        /// <summary>
        /// Delete Booking by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteService(short id)
        {
            return Ok(await Mediator.Send(new DeleteBookingCommand
            {
                Id = id
            }));
        }
    }
}

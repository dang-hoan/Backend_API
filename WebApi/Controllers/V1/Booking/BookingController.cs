using Application.Features.Booking.Queries.GetAll;
using Domain.Wrappers;
﻿using Application.Features.Booking.Command.AddBooking;
using Application.Features.Booking.Queries.GetById;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Booking
{
    [ApiController]
    [Route("api/v{version:apiVersion}/booking")]
    public class BookingController : BaseApiController<BookingController>
    {
        /// Get all bookings pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        // [Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllBookingResponse>>> GetAllBookings([FromQuery] GetAllBookingQuery query)
        {
            return Ok(await Mediator.Send(new GetAllBookingQuery()
            {
                IsExport = query.IsExport,
                Keyword = query.Keyword,
                SortBy = query.SortBy,
                OrderBy = query.OrderBy,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
                BookingDate = query.BookingDate,
                FromTime = query.FromTime,
                Totime = query.Totime,
                Status = query.Status
            }));
        }
        
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
    }
}

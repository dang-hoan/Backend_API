using Application.Features.BookingStatusEnum.Command.AddBookingStatus;
using Application.Features.BookingStatusEnum.Command.DeleteBookingStatus;
using Application.Features.BookingStatusEnum.Command.EditBookingStatus;
using Application.Features.BookingStatusEnum.Queries.GetAll;
using Application.Features.BookingStatusEnum.Queries.GetById;
using Domain.Constants;
using Domain.Wrappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.BookingStatusEnum
{
    [ApiController]
    [Route("api/v{version:apiVersion}/bookingstatus")]
    public class BookingStatusController : BaseApiController<BookingStatusController>
    {
        /// <summary>
        /// Get all booking statuses pagination, filter
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllBookingStatusResponse>>> GetAllBookingStatuses([FromQuery] GetAllBookingStatusQuery query)
        {
            return Ok(await Mediator.Send(new GetAllBookingStatusQuery()
            {
                IsExport = query.IsExport,
                Keyword = query.Keyword,
                OrderBy = query.OrderBy,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            }));
        }

        /// <summary>
        /// Add Booking Status
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpPost]
        public async Task<IActionResult> AddBookingStatus(AddBookingStatusCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Get Booking Status Detail
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpGet("{Id}")]
        public async Task<IActionResult> BookingStatusDetail(int Id)
        {
            var result = await Mediator.Send(new GetBookingStatusByIdQuery
            {
                Id = Id
            });
            return Ok(result);
        }

        /// <summary>
        /// Delete Booking Status by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpDelete]
        public async Task<IActionResult> DeleteBookingStatus(int id)
        {
            var result = await Mediator.Send(new DeleteBookingStatusCommand
            {
                Id = id
            });
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Edit Booking Status
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpPut]
        public async Task<IActionResult> EditBookingStatus(EditBookingStatusCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
    }
}
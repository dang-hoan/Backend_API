using Application.Features.Customer.Command.AddCustomer;
using Application.Features.Customer.Queries.GetById;
using Domain.Wrappers;
﻿using Application.Features.Cusomter.Queries.GetAll;
using Application.Features.Customer.Queries.GetAll;
using Microsoft.AspNetCore.Authorization;
using Application.Features.Customer.Command.EditCustomer;
using Application.Features.Customer.Queries.GetCustomerBookingHistory;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.Customer
{
    [ApiController]

    [Route("api/v{version:apiVersion}/customer")]
    public class CustomerController : BaseApiController<CustomerController>
    {
        /// <summary>
        /// Get Customer detail by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Result<GetCustomerByIdResponse>>> GetCustomerById(long id)
        {
            return Ok(await Mediator.Send(new GetCustomerByIdQuery()
            {
                Id = id
            }));
        }

        /// <summary>
        /// Add/Edit customer
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPost]
        public async Task<IActionResult> AddCustomer(AddCustomerCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Succeeded == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        /// Get all customers pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        // [Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllCustomerResponse>>> GetAllCustomer([FromQuery] GetAllCustomerQuery query)
        {
            return Ok(await Mediator.Send(new GetAllCustomerQuery()
            {
                IsExport = query.IsExport,
                Keyword = query.Keyword,
                SortBy = query.SortBy,
                OrderBy = query.OrderBy,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize
            }));
        }

        /// <summary>
        /// Edit Customer
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpPut]
        public async Task<IActionResult> EditCustomer(EditCustomerCommand command)
        {
            var result = await Mediator.Send(command);
            if (result.Succeeded == false)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        /// <summary>
        /// Get customer booking history
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("{CustomerId}/get-booking-history")]
        public async Task<IActionResult> GetCustomerBookingHistory(long CustomerId)
        {
            return Ok(await Mediator.Send(new GetCustomerBookingHistoryQuery
            {
                CustomerId = CustomerId
            }));
        }
    }
}

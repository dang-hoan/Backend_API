using Application.Features.Customer.Command.AddCustomer;
using Application.Features.Customer.Queries.GetById;
using Domain.Wrappers;
﻿using Application.Features.Cusomter.Queries.GetAll;
using Application.Features.Customer.Queries.GetAll;
using Microsoft.AspNetCore.Authorization;
using Application.Features.Customer.Command.EditCustomer;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Customer.Command.DeleteCustomer;

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
        /// Delete customer
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(long Id)
        {
            return Ok(await Mediator.Send(new DeleteCustomerCommand
            {
                Id = Id
            }));
        }
    }
}

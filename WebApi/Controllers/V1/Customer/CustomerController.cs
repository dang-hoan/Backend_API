using Application.Features.Customer.Command.AddCustomer;
using Application.Features.Customer.Queries.GetById;
using Domain.Wrappers;
﻿using Application.Features.Cusomter.Queries.GetAll;
using Application.Features.Customer.Queries.GetAll;
using Microsoft.AspNetCore.Authorization;
using Application.Features.Customer.Command.EditCustomer;
using Microsoft.AspNetCore.Mvc;
using Application.Features.Customer.Command.DeleteCustomer;
using Domain.Constants;

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
        [Authorize]
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
        [HttpPost]
        public async Task<IActionResult> AddCustomer(AddCustomerCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }

        /// Get all customers pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdminAndEmployeeRole)]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllCustomerResponse>>> GetAllCustomer([FromQuery] GetAllCustomerQuery query)
        {
            return Ok(await Mediator.Send(new GetAllCustomerQuery()
            {
                IsExport = query.IsExport,
                Keyword = query.Keyword,
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
        [Authorize]
        [HttpPut]
        public async Task<IActionResult> EditCustomer(EditCustomerCommand command)
        {
            var result = await Mediator.Send(command);
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
        /// <summary>
        /// Delete customer
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles =RoleConstants.AdminAndEmployeeRole)]
        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(long Id)
        {
            var result = await Mediator.Send(new DeleteCustomerCommand
            {
                Id = Id
            });
            return (result.Succeeded) ? Ok(result) : BadRequest(result);
        }
    }
}

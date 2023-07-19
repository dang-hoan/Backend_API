using Application.Features.Employee.Queries.GetAll;
using Application.Features.WorkShift.Queries.GetAll;
using Application.Parameters;
using Domain.Wrappers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.V1.WorkShift
{
    [ApiController]

    [Route("api/v{version:apiVersion}/workshift")]
    public class WorkShiftController : BaseApiController<WorkShiftController>
    {
        /// Get all Workshift pagination, filter
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<PaginatedResult<GetAllEmployeeResponse>>> GetAllWorkShift([FromQuery] RequestParameter parameter)
        {
            return Ok(await Mediator.Send(new GetAllWorkShiftQuery()
            {
                IsExport = parameter.IsExport,
                Keyword = parameter.Keyword,
                OrderBy = parameter.OrderBy,
                PageNumber = parameter.PageNumber,
                PageSize = parameter.PageSize,
            }));
        }
    }
}

using Domain.Wrappers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Domain.Constants.Enum;
using Application.Features.Statistics.Queries.GetInsightMetrics;
using Application.Features.Statistics.Queries.GetOverview;
using Application.Features.Statistics.Queries.GetOustandingService;
using Domain.Constants;

namespace WebApi.Controllers.V1.Statistics
{
    [ApiController]
    [Route("api/v{version:apiVersion}/statistics")]
    public class StatisticsController : BaseApiController<StatisticsController>
    {
        /// <summary>
        /// Get Insight Metrics
        /// </summary>
        /// <param name="statisticsTime"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpGet("insight-metrics")]
        public async Task<ActionResult<Result<GetInsightMetricsResponse>>> GetInsightMetrics(StatisticsTime statisticsTime)
        {
            return Ok(await Mediator.Send(new GetInsightMetricsQuery()
            {
                statisticsTime = statisticsTime
            }));
        }
        /// <summary>
        /// Get Overview
        /// </summary>
        /// <param name="statisticsTime"></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpGet("overview")]
        public async Task<ActionResult<Result<List<GetOverviewResponse>>>> GetOverView(StatisticsTime statisticsTime)
        {
            return Ok(await Mediator.Send(new GetOverviewQuery()
            {
                statisticsTime = statisticsTime
            }));
        }
        /// <summary>
        /// Get Outstanding service
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [Authorize(Roles = RoleConstants.AdministratorRole)]
        [HttpGet("outstanding-service")]
        public async Task<ActionResult<Result<List<GetOverviewResponse>>>> GetOutstandingService()
        {
            return Ok(await Mediator.Send(new GetOutstandingServiceQuery()
            {
            }));
        }
    }
}


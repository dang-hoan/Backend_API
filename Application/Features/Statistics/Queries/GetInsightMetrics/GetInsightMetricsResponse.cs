using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Statistics.Queries.GetInsightMetrics
{
    public class GetInsightMetricsResponse
    {
        public long Subscription { get; set; }
        public decimal Revenue { get; set; }
        public decimal Sales { get; set; }
        public long Feedback { get; set; }
    }
}

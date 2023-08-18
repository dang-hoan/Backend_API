using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Statistics.Queries.GetOverview
{
    public class GetOverviewResponse
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public long Reach { get; set; }
    }
}

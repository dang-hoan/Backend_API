using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Statistics.Queries.GetOustandingService
{
    public class GetOutstandingServiceResponse
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Revenue { get; set; }
    }
}

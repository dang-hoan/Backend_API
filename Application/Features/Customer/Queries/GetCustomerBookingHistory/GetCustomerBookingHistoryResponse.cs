using Domain.Constants.Enum;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Customer.Queries.GetCustomerBookingHistory
{
    public class GetCustomerBookingHistoryResponse
    {
        public List<CustomerBookingHistoryResponse> CustomerBookingHistorys { get; set; } = new List<CustomerBookingHistoryResponse>();

    }
    public class CustomerBookingHistoryResponse
    {
        public long CustomerId { get; set; }
        public long BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public BookingStatus? Status { get; set; }
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal price { get; set; }
    }
}

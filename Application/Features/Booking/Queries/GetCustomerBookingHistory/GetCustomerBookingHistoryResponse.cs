using Domain.Constants.Enum;

namespace Application.Features.Booking.Queries.GetCustomerBookingHistory
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

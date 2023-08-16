using Application.Dtos.Responses.ServiceImage;

namespace Application.Features.Booking.Queries.GetCustomerBooking
{
    public class GetCustomerBookingResponse
    {
        public long BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public int? BookingStatus { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public List<BookingDetailResponse> bookingDetailResponses { get; set; } = new List<BookingDetailResponse>();
    }
    public class BookingDetailResponse
    {
        public long BookingDetailId { get; set; }
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string? ServiceDescription { get; set; }
        public decimal ServicePrice { get; set; }
        public List<ServiceImageResponse> ServiceImages { get; set; }
    }
}

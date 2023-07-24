using Application.Dtos.Responses.ServiceImage;
using Domain.Constants.Enum;

namespace Application.Features.Booking.Queries.GetCustomerBooking
{
    public class GetCustomerBookingResponse
    {
       public List<BookingResponse> bookings { get; set; } = new List<BookingResponse>();
    }
    public class BookingResponse
    {
        public long BookingId { get; set; }
        public DateTime BookingDate { get; set; }
        public BookingStatus? BookingStatus { get; set; }
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

using Application.Parameters;

namespace Application.Features.Booking.Queries.GetAll
{
    public class GetAllBookingParameter : RequestParameter
    {
        public DateTime? BookingDate { get; set; }

        public DateTime? UseTime { get; set; }

        public int? Status { get; set; }

    }
}

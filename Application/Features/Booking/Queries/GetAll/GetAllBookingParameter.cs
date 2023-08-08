using Application.Parameters;
using Domain.Constants.Enum;

namespace Application.Features.Booking.Queries.GetAll
{
    public class GetAllBookingParameter : RequestParameter
    {
        public DateTime? BookingDate { get; set; }

        public DateTime? FromTime { get; set; }

        public DateTime? ToTime { get; set; }

        public BookingStatus? Status { get; set; }

    }
}

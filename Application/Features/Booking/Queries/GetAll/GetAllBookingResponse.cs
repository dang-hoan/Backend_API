namespace Application.Features.Booking.Queries.GetAll
{
    public class GetAllBookingResponse
    {
        public long Id { get; set; }

        public string CustomerName { get; set; }

        public string PhoneNumber { get; set; }

        public DateTime BookingDate { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public int? Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? LastModifiedOn { get; set; }
    }
}

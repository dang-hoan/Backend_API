namespace Application.Features.Booking.Queries.GetById
{
    public class GetBookingByIdResponse
    {
        public long Id { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public int? Status { get; set; }
        public DateTime FromTime { get; set; }
        public DateTime ToTime { get; set; }
        public string? Note { get; set; }
        public List<ServiceBookingResponse> Services { get; set; } = new List<ServiceBookingResponse>();
    }

    public class ServiceBookingResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int ServiceTime { get; set; }
    }
}
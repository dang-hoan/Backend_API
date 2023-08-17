namespace Application.Features.BookingStatusEnum.Queries.GetAll
{
    public class GetAllBookingStatusResponse
    {
        public int Id { get; set; }

        public string Value { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? LastModifiedOn { get; set; }
    }
}

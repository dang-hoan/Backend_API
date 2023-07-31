namespace Application.Features.Customer.Queries.GetById
{
    public class GetCustomerByIdResponse
    {
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public decimal? TotalMoney { get; set; }
    }
}
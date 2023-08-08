namespace Application.Features.Employee.Queries.GetById
{
    public class GetEmployeeByIdResponse
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string Email { get; set; }
        public bool? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string? Image { get; set; }
        public string? ImageLink { get; set; }
        public string? UserName { get; set; }
        public long WorkShiftId { get; set; }
    }
}
namespace Application.Features.Employee.Queries.GetAll
{
    public class GetAllEmployeeResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool? Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public long WorkShiftId { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string? ImageFile { get; set; }
        public string? ImageLink { get; set; }
    }
}
namespace Application.Features.WorkShift.Queries.GetById
{
    public class GetWorkshiftByIdResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public double TimeWork { get; set; }
        public bool? IsDefault { get; set; }
        public string? Description { get; set; }
        public List<int>? WorkDays { get; set; }
    }
}

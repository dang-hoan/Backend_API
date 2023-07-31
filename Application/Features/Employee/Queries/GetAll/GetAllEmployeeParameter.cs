using Application.Parameters;

namespace Application.Features.Employee.Queries.GetAll
{
    public class GetAllEmployeeParameter : RequestParameter
    {
        public long? WorkShiftId { get; set; }
        public bool? Gender { get; set; }

    }
}

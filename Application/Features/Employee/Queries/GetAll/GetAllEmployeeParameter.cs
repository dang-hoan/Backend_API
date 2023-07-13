using Application.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employee.Queries.GetAll
{
    public class GetAllEmployeeParameter : RequestParameter
    {
        public long? WorkShiftId { get; set; }
        public bool? Gender { get; set; }

    }
}

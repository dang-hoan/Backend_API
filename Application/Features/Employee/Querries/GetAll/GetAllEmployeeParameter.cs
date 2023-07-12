using Application.Parameters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employee.Querries.GetAll
{
    public class GetAllEmployeeParameter : RequestParameter
    {
        public DateTime? MaxBirthDay { get; set; }
        public DateTime? MinBirthDay { get; set; }
        public bool? Gender { get; set; }

    }
}

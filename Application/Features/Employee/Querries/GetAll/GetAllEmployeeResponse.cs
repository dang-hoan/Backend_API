using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Employee.Querries.GetAll
{
    public class GetAllEmployeeResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}

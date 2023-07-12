using Application.Features.Employee.Command.AddEmployee;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Mappings.Employee
{
    public class EmployeeMappings : Profile
    {
        public EmployeeMappings()
        {
            CreateMap<AddEmployeeCommand, Domain.Entities.Employee.Employee>().ReverseMap();
        }
    }
}

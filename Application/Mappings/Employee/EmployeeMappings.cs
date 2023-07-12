using Application.Features.Employee.Command.AddEmployee;
using AutoMapper;

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

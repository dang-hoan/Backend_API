using Application.Features.Employee.Queries.GetById;
﻿using Application.Features.Employee.Command.AddEmployee;
using AutoMapper;

namespace Application.Mappings.Employee
{
    public class EmployeeMappings : Profile
    {
        public EmployeeMappings()
        {
            
            CreateMap<Domain.Entities.Employee.Employee, GetEmployeeByIdQuery>().ReverseMap();
            CreateMap<AddEmployeeCommand, Domain.Entities.Employee.Employee>().ReverseMap();
        }
    }
}



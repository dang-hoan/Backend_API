using Application.Features.Employee.Queries.GetById;
﻿using Application.Features.Employee.Command.AddEmployee;
using Application.Features.Employee.Command.EditEmployee;
using AutoMapper;

namespace Application.Mappings.Employee
{
    public class EmployeeMappings : Profile
    {
        public EmployeeMappings()
        {
            
            CreateMap<Domain.Entities.Employee.Employee, GetEmployeeByIdQuery>().ReverseMap();
            CreateMap<AddEmployeeCommand, Domain.Entities.Employee.Employee>().ReverseMap();
            CreateMap<EditEmployeeCommand, Domain.Entities.Employee.Employee>()
                .ForMember(dest => dest.Image, opt => opt.Condition(src => src.ImageFile != null))
                .ReverseMap();
        }
    }
}



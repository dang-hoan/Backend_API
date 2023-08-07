using Application.Features.Employee.Command.AddEmployee;
using Application.Features.Employee.Command.EditEmployee;
using Application.Features.Employee.Queries.GetById;
using AutoMapper;
using Domain.Constants.Enum;
using Domain.Entities;

namespace Application.Mappings.Employee
{
    public class EmployeeMappings : Profile
    {
        public EmployeeMappings()
        {
            CreateMap<Domain.Entities.Employee.Employee, GetEmployeeByIdQuery>().ReverseMap();
            CreateMap<AddEmployeeCommand, Domain.Entities.Employee.Employee>().ReverseMap();
            CreateMap<AddEmployeeCommand, AppUser>()
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.CreatedOn, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.TypeFlag, opt => opt.MapFrom(src => TypeFlagEnum.Employee))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.Image))
                .ReverseMap();
            CreateMap<EditEmployeeCommand, Domain.Entities.Employee.Employee>()
                .ReverseMap();
        }
    }
}
using Application.Features.Customer.Command.AddCustomer;
using Application.Features.Customer.Command.EditCustomer;
using AutoMapper;

namespace Application.Mappings.Customer
{
    public class CustomerMappings : Profile
    {
        public CustomerMappings() 
        {
            CreateMap<AddCustomerCommand,Domain.Entities.Customer.Customer>().ReverseMap();
            CreateMap<EditCustomerCommand,Domain.Entities.Customer.Customer>()
                .ForMember(dest => dest.TotalMoney, opt => opt.Ignore()).ReverseMap();
        }

    }
}

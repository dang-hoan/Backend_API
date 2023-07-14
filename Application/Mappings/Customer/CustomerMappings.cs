using Application.Features.Customer.Command.AddCustomer;
using AutoMapper;

namespace Application.Mappings.Customer
{
    public class CustomerMappings : Profile
    {
        public CustomerMappings() 
        {
            CreateMap<AddCustomerCommand,Domain.Entities.Customer.Customer>().ReverseMap();
        }

    }
}

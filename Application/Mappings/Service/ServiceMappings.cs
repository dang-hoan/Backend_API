using Application.Features.Service.Command.AddService;
using AutoMapper;

namespace Application.Mappings.Service
{
    public class ServiceMappings : Profile
    {
        public ServiceMappings()
        {
            CreateMap<AddServiceCommand, Domain.Entities.Service.Service>().ReverseMap();
        }
    }
}

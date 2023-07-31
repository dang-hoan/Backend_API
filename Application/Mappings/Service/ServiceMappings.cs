using Application.Features.Service.Command.AddService;
using Application.Features.Service.Command.EditService;
using AutoMapper;

namespace Application.Mappings.Service
{
    public class ServiceMappings : Profile
    {
        public ServiceMappings()
        {
            CreateMap<AddServiceCommand, Domain.Entities.Service.Service>().ReverseMap();
            CreateMap<EditServiceCommand, Domain.Entities.Service.Service>().ReverseMap();
        }
    }
}

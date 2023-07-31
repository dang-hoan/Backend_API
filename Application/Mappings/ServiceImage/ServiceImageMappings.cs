using Application.Dtos.Responses.ServiceImage;
using AutoMapper;

namespace Application.Mappings.ServiceImage
{
    public class ServiceImageMappings : Profile
    {
        public ServiceImageMappings()
        {
            CreateMap<Domain.Entities.ServiceImage.ServiceImage, ServiceImageResponse>().ReverseMap();
        }
    }
}

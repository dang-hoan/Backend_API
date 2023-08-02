using Application.Dtos.Requests.Feedback;
using Application.Dtos.Responses.ServiceImage;
using AutoMapper;
using Domain.Entities.ServiceImage;

namespace Application.Mappings.ServiceImages
{
    public class ServiceImageMappings : Profile
    {
        public ServiceImageMappings()
        {
            CreateMap<ServiceImage, ServiceImageResponse>().ReverseMap();
            CreateMap<ServiceImage, ImageRequest>().ReverseMap();
        }
    }
}
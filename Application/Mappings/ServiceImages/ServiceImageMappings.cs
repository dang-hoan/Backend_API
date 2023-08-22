using Application.Dtos.Requests.Feedback;
using AutoMapper;
using Domain.Entities.ServiceImage;

namespace Application.Mappings.ServiceImages
{
    public class ServiceImageMappings : Profile
    {
        public ServiceImageMappings()
        {
            CreateMap<ServiceImage, ImageRequest>().ReverseMap();
        }
    }
}
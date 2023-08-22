using Application.Dtos.Responses.ServiceImage;
using Application.Mappings.CustomConverter;
using AutoMapper;
using Domain.Entities.ServiceImage;
using Microsoft.AspNetCore.Http;

namespace Application.Mappings
{
    public class CustomMapping : Profile
    {
        public CustomMapping() 
        {
            CreateMap<IFormFile, byte[]>().ConvertUsing<IFormFileToByteArrayConverter>();
            CreateMap<ServiceImage, ServiceImageResponse>().ConvertUsing<ServiceImageToServiceImageResponseConverter>();
        }
        
    }
}

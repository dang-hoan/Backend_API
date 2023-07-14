using Application.Mappings.CustomConverter;
using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace Application.Mappings
{
    public class CustomMapping : Profile
    {
        public CustomMapping() 
        {
            CreateMap<IFormFile, byte[]>().ConvertUsing<IFormFileToByteArrayConverter>();
        }
        
    }
}

using AutoMapper;
using Microsoft.AspNetCore.Http;

namespace Application.Mappings.CustomConverter
{
    public class IFormFileToByteArrayConverter : ITypeConverter<IFormFile, byte[]>
    {
        public byte[] Convert(IFormFile source, byte[] destination, ResolutionContext context)
        {
            using (var memoryStream = new MemoryStream())
            {
                source.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}

using Application.Dtos.Responses.ServiceImage;
using Application.Interfaces;
using AutoMapper;
using Domain.Entities.ServiceImage;

namespace Application.Mappings.CustomConverter
{
    public class ServiceImageToServiceImageResponseConverter : ITypeConverter<ServiceImage, ServiceImageResponse>
    {
        private readonly IUploadService _uploadService;
        public ServiceImageToServiceImageResponseConverter(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }
        public ServiceImageResponse Convert(ServiceImage source, ServiceImageResponse destination, ResolutionContext context)
        {
            return new ServiceImageResponse
            {
                Id = source.Id,
                ServiceId = source.ServiceId,
                NameFile = source.NameFile,
                NameFileLink = _uploadService.GetFullUrl(source.NameFile),
            };
        }
    }
}

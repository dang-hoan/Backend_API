using Application.Interfaces.Repositories;

namespace Application.Interfaces.ServiceImage
{
    public interface IServiceImageRepository : IRepositoryAsync<Domain.Entities.ServiceImage.ServiceImage, long>
    {
    }
}

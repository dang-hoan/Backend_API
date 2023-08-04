using Application.Interfaces.ServiceImage;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.ServiceImage
{
    public class ServiceImageRepository : RepositoryAsync<Domain.Entities.ServiceImage.ServiceImage, long>, IServiceImageRepository
    {
        public ServiceImageRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
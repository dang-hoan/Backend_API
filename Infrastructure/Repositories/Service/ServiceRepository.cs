using Application.Interfaces.Service;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.Service
{
    public class ServiceRepository : RepositoryAsync<Domain.Entities.Service.Service, long>, IServiceRepository
    {
        public ServiceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
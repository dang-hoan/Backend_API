using Application.Interfaces.Repositories;

namespace Application.Interfaces.Service
{
    public interface IServiceRepository : IRepositoryAsync<Domain.Entities.Service.Service, long>
    {
    }
}

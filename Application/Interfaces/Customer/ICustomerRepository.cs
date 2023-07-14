using Application.Interfaces.Repositories;

namespace Application.Interfaces.Customer
{
    public interface ICustomerRepository : IRepositoryAsync<Domain.Entities.Customer.Customer, long>
    {
    }
}

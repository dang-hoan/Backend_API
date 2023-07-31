using Application.Interfaces.Customer;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.Customer
{
    public class CustomerRepository : RepositoryAsync<Domain.Entities.Customer.Customer, long>, ICustomerRepository
    {
        public CustomerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}

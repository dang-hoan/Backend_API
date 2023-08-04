using Application.Interfaces.Employee;
using Infrastructure.Contexts;

namespace Infrastructure.Repositories.Employee
{
    public class EmployeeRepository : RepositoryAsync<Domain.Entities.Employee.Employee, long>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
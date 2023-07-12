using Application.Interfaces.Repositories;
using Domain.Entities.Employee;

namespace Application.Interfaces.Employee
{
    public interface IEmployeeRepository : IRepositoryAsync<Domain.Entities.Employee.Employee, long>
    {
    }
}

using Application.Interfaces.Repositories;

namespace Application.Interfaces.Employee
{
    public interface IEmployeeRepository : IRepositoryAsync<Domain.Entities.Employee.Employee, long>
    {
    }
}

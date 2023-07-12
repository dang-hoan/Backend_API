using Application.Interfaces.Employee;
using Infrastructure.Repositories.EmployeeRepo;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class AddRepositoryExtensions
    {
        public static void AddEmployeeRepository(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        }
    }
}

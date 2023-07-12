using Application.Interfaces.Employee;
using Infrastructure.Repositories.Employee;
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

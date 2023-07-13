using Application.Interfaces.Employee;
using Application.Interfaces.Service;
using Infrastructure.Repositories.Employee;
using Infrastructure.Repositories.Service;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions
{
    public static class AddRepositoryExtensions
    {
        public static void AddEmployeeRepository(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        }
        public static void AddServiceRepository(this IServiceCollection services)
        {
            services.AddScoped<IServiceRepository, ServiceRepository>();
        }
    }
}

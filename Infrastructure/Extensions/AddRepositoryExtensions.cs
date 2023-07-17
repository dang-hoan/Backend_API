using Application.Interfaces.Customer;
using Application.Interfaces.Employee;
using Application.Interfaces.Feedback;
using Application.Interfaces.Service;
using Application.Interfaces.ServiceImage;
using Infrastructure.Repositories.Customer;
using Infrastructure.Repositories.Employee;
using Infrastructure.Repositories.Feedback;
using Infrastructure.Repositories.Service;
using Infrastructure.Repositories.ServiceImage;
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
        public static void AddServiceImageRepository(this IServiceCollection services)
        {
            services.AddScoped<IServiceImageRepository, ServiceImageRepository>();
        }
        public static void AddCustomerRepository(this IServiceCollection services)
        {
            services.AddScoped<ICustomerRepository, CustomerRepository>();
        }
        public static void AddFeedbackRepository(this IServiceCollection services)
        {
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
        }
    }
}

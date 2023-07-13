using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Services.Account;
using Application.Interfaces.Services.Identity;
using Infrastructure.Contexts;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Infrastructure.Extensions;

namespace Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), o =>
                {
                    o.EnableRetryOnFailure();
                    o.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddTransient(typeof(IRepositoryAsync<,>), typeof(RepositoryAsync<,>));
            services.AddTransient(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddTransient<IDatabaseSeeder, DatabaseSeeder>();

            services.AddScoped<ITokenService, IdentityService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IUploadService, UploadService>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddEmployeeRepository();
            services.AddServiceRepository();
        }

    }
}
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
                    o.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                });
                //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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
            services.AddTransient<IUploadService, UploadService>();
            services.AddScoped<IRemoveImageService, RemoveImageService>();
            services.AddScoped<ICheckFileType, CheckFileType>();
            services.AddScoped<ICheckSizeFile, CheckFileSize>();
            services.AddScoped<IEnumService, EnumService>();
        }

        public static void AddRepositories(this IServiceCollection services)
        {
            services.AddEmployeeRepository();
            services.AddServiceRepository();
            services.AddServiceImageRepository();
            services.AddCustomerRepository();
            services.AddBookingRepository();
            services.AddBookingDetailRepository();
            services.AddViewCustomerBookingHistoryRepository();
            services.AddFeedbackRepository();
            services.AddReplyRepository();
            services.AddViewCustomerFeedbackReplyRepository();
            services.AddWorkShiftRepository();
            services.AddViewCustomerReviewHistoryRepository();
            services.AddFeedbackFileUploadRepository();
            services.AddEnumMasterDataRepository();
        }

    }
}
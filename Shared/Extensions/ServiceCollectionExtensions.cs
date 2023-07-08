using Application.Interfaces;
using Application.Interfaces.Services;
using ERP.Shared.Configurations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Shared.Configurations;
using Shared.Services;

namespace Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSharedExtensions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AppConfiguration>(configuration.GetSection(nameof(AppConfiguration)));
            services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
            services.Configure<MailConfiguration>(configuration.GetSection(nameof(MailConfiguration)));
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDateTimeService, SystemDateTimeService>();
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
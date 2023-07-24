using Application.Enums;
using Application.Exceptions;
using Application.Interfaces.Services;
using Hangfire;
using Microsoft.Extensions.FileProviders;
using WebApi.Filters;
using WebApi.Middlewares;

namespace WebApi.Extensions
{
    public static class AppExtensions
    {

        public static void UseFolderAsStatic(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            string folderPath = Path.Combine("Files", new UploadType().ToDescriptionString());
            folderPath = folderPath.Replace('\\', '/');

            app.UseStaticFiles();  // Enables the serving of static files

            // Create directory to save file if not exists
            string pathToSave = Path.Combine(env.ContentRootPath, folderPath);
            bool exists = System.IO.Directory.Exists(pathToSave);
            if (!exists)
                System.IO.Directory.CreateDirectory(pathToSave);

            // Map the directory where your images are saved
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
                    pathToSave),
                RequestPath = "/" + folderPath
            });
        }

        public static void UseSwaggerExtension(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint("/swagger/v1/swagger.json", "CleanArchitecture.Source.WebApi");
            });
        }

        public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }

        public static void UseHangfireExtension(this IApplicationBuilder app)
        {
            app.UseHangfireDashboard("/jobs", new DashboardOptions
            {
                DashboardTitle = "NinePlus.ERP",
                Authorization = new[] { new HangfireAuthorizationFilter() }
            });
        }

        public static IApplicationBuilder InitializeDb(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.CreateScope();

            var initializers = serviceScope.ServiceProvider.GetServices<IDatabaseSeeder>();

            foreach (var initializer in initializers)
            {
                initializer.Initialize();
            }

            return app;
        }
    }
}
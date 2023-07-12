using Application.Extensions;
using Infrastructure.Extensions;
using Serilog;
using Shared.Extensions;
using WebApi.Extensions;

Log.Logger = new LoggerConfiguration()
                 .WriteTo.Console()
                 .CreateBootstrapLogger();
var builder = WebApplication.CreateBuilder(args);
Log.Information($"Start {builder.Environment.ApplicationName} up");
// Add services to the container.
try
{
    builder.Host.AddAppConfigurations();

    builder.Services.AddRepositories();

    builder.Services.AddApplicationExtensions();

    builder.Services.AddPersistenceInfrastructure(builder.Configuration);

    builder.Services.AddSharedExtensions(builder.Configuration);

    builder.Services.AddSwaggerExtension();

    builder.Services.AddApiversioningExtension();

    builder.Services.AddCorsExtensions();

    builder.Services.AddIdentityServices();

    builder.Services.AddJwtAuthentication(builder.Services.GetApplicationSettings(builder.Configuration));

    builder.Services.AddCurrentUserService();

    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();

    builder.Services.AddLazyCache();

    var app = builder.Build();

    app.UseRouting();

    app.UseStaticFiles();

    app.UseCors("CorsPolicy");

    app.UseSwaggerExtension();

    //app.UseHangfireExtension();

    app.UseErrorHandlingMiddleware();

    app.UseAuthentication();

    app.UseAuthorization();

    //app.UseHealthChecks("/health");

    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
    });

    // Seeding data
    app.InitializeDb();

    app.Run();
}
catch (Exception ex)
{
    string type = ex.GetType().Name;
    if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

    Log.Fatal(ex, $"Unhandled exception: {ex.Message}");
}
finally
{
    Log.Information($"Shut down {builder.Environment.ApplicationName} complete");
    Log.CloseAndFlush();
}
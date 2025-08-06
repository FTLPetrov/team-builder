using TeamBuilder.WebApi.Configuration;

namespace TeamBuilder.WebApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services
                .AddApplicationServices(builder.Configuration)
                .AddAuthenticationServices(builder.Configuration)
                .AddDatabaseServices(builder.Configuration)
                .AddRepositoryServices()
                .AddApplicationServices();

            var app = builder.Build();


            await StartupConfiguration.ConfigureStartupServices(app);


            MiddlewareConfiguration.ConfigureDevelopmentServices(app);


            MiddlewareConfiguration.ConfigureSecurityMiddleware(app);


            MiddlewareConfiguration.ConfigureStandardMiddleware(app);


            MiddlewareConfiguration.ConfigureEndpoints(app);

            app.Run();
        }
    }
}

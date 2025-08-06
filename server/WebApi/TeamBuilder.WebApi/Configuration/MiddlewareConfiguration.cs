using TeamBuilder.WebApi.Middleware;
using TeamBuilder.WebApi.Hubs;

namespace TeamBuilder.WebApi.Configuration
{
    public static class MiddlewareConfiguration
    {
        public static void ConfigureDevelopmentServices(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }

        public static void ConfigureSecurityMiddleware(WebApplication app)
        {
            // Add global error handling middleware (should be first)
            app.UseMiddleware<ErrorHandlingMiddleware>();

            // Add security headers middleware
            app.UseMiddleware<SecurityHeadersMiddleware>();

            // Use CORS
            app.UseCors(ApplicationConstants.Cors.PolicyName);
        }

        public static void ConfigureStandardMiddleware(WebApplication app)
        {
            // Serve static files
            app.UseStaticFiles();

            // Add security middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();
        }

        public static void ConfigureEndpoints(WebApplication app)
        {
            app.MapControllers();
            
            // Map SignalR hub
            app.MapHub<ChatHub>(ApplicationConstants.SignalR.ChatHubPath);
        }
    }
}

using TeamBuilder.Data;
using TeamBuilder.Data.Models;
using Microsoft.AspNetCore.Identity;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Data.Common.Security;

namespace TeamBuilder.WebApi.Configuration
{
    public static class StartupConfiguration
    {
        public static async Task ConfigureStartupServices(WebApplication app)
        {

            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TeamBuilderDbContext>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                await SeedAdminUser(context, userManager);
            }
        }

        private static async Task SeedAdminUser(TeamBuilderDbContext context, UserManager<User> userManager)
        {
            try
            {

                var adminEmail = SecurityConfig.Admin.DefaultEmail;
                var adminUsername = SecurityConfig.Admin.DefaultUsername;
                var adminFirstName = SecurityConfig.Admin.DefaultFirstName;
                var adminLastName = SecurityConfig.Admin.DefaultLastName;
                var adminPassword = SecurityConfig.Admin.DefaultPassword;


                var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
                
                if (existingAdmin == null)
                {

                    var adminUser = new User(adminEmail, adminUsername, adminFirstName, adminLastName);
                    adminUser.IsAdmin = true;
                    adminUser.EmailConfirmed = true;
                    adminUser.IsDeleted = false;

                    var result = await userManager.CreateAsync(adminUser, adminPassword);
                    
                    if (result.Succeeded)
                    {

                    }
                    else
                    {
                        var errorMessage = string.Join(", ", result.Errors.Select(e => e.Description));

                    }
                }
                else
                {

                    existingAdmin.IsAdmin = true;
                    existingAdmin.EmailConfirmed = true;
                    existingAdmin.IsDeleted = false;
                    await userManager.UpdateAsync(existingAdmin);
                    

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}

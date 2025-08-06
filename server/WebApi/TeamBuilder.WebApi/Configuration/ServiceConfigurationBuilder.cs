using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TeamBuilder.Data;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories;
using TeamBuilder.Data.Repositories.Interfaces;
using TeamBuilder.Services.Core;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Services.Core.Services;
using TeamBuilder.Data.Common.Security;

namespace TeamBuilder.WebApi.Configuration
{
    public static class ServiceConfigurationBuilder
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Initialize SecurityConfig with configuration
            SecurityConfig.Initialize(configuration);

            // Add Controllers and API Explorer
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // Add SignalR
            services.AddSignalR();

            // Add CORS
            services.AddCors(options =>
            {
                options.AddPolicy(ApplicationConstants.Cors.PolicyName, policy =>
                {
                    policy.WithOrigins(SecurityConfig.CORS.AllowedOrigins)
                          .WithHeaders(SecurityConfig.CORS.AllowedHeaders)
                          .WithMethods(SecurityConfig.CORS.AllowedMethods)
                          .WithExposedHeaders(SecurityConfig.CORS.ExposedHeaders)
                          .AllowCredentials();
                });
            });

            // Configure Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(ApplicationConstants.Swagger.Version, new OpenApiInfo { 
                    Title = ApplicationConstants.Swagger.Title, 
                    Version = ApplicationConstants.Swagger.Version 
                });
                
                c.AddSecurityDefinition(ApplicationConstants.Swagger.BearerScheme, new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = ApplicationConstants.Swagger.AuthorizationHeader,
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = ApplicationConstants.Swagger.BearerScheme
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = ApplicationConstants.Swagger.BearerScheme
                            }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }

        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
                        ValidateIssuer = true,
                        ValidIssuer = issuer,
                        ValidateAudience = true,
                        ValidAudience = audience,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    
                    // Configure JWT for SignalR
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            
                            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments(ApplicationConstants.SignalR.ChatHubPath))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            // Register Identity with custom password requirements
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = SecurityConfig.PasswordPolicy.MinLength;
                options.Password.RequireUppercase = SecurityConfig.PasswordPolicy.RequireUppercase;
                options.Password.RequireLowercase = SecurityConfig.PasswordPolicy.RequireLowercase;
                options.Password.RequireDigit = SecurityConfig.PasswordPolicy.RequireDigit;
                options.Password.RequireNonAlphanumeric = SecurityConfig.PasswordPolicy.RequireSpecialCharacter;
            });

            services.AddIdentity<User, IdentityRole<Guid>>()
                .AddEntityFrameworkStores<TeamBuilderDbContext>();
            
            // Add Authorization
            services.AddAuthorization();

            return services;
        }

        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Database Context
            services.AddDbContext<TeamBuilderDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
                    b => b.MigrationsAssembly("TeamBuilder.Data")));

            return services;
        }

        public static IServiceCollection AddRepositoryServices(this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<ITeamMemberRepository, TeamMemberRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventParticipationRepository, EventParticipationRepository>();
            services.AddScoped<IInvitationRepository, InvitationRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<ISupportMessageRepository, SupportMessageRepository>();
            services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();
            services.AddScoped<IWarningRepository, WarningRepository>();

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Application Services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ITeamService, TeamService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IInvitationService, InvitationService>();
            services.AddScoped<IChatService, TeamBuilder.Services.Core.Services.ChatService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IValidationService, ValidationService>();

            return services;
        }
    }
}

using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TeamBuilder.Services.Core.Interfaces;

namespace TeamBuilder.WebApi.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IJwtService jwtService, IUserService userService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var principal = jwtService.ValidateToken(token);
                    if (principal != null)
                    {
                        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
                        {
                            var user = await userService.GetByIdAsync(userGuid);
                            if (user != null)
                            {
                                context.Items["User"] = user;
                                context.Items["UserId"] = userGuid;
                            }
                        }
                    }
                }
                catch
                {
                    // Token validation failed, but we continue without authentication
                }
            }

            await _next(context);
        }
    }
} 
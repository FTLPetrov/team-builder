using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using TeamBuilder.Services.Core.Interfaces;
using System.Collections.Generic;

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
                Console.WriteLine($"JWT Middleware: Processing token: {token.Substring(0, Math.Min(20, token.Length))}...");
                try
                {
                    var principal = jwtService.ValidateToken(token);
                    if (principal != null)
                    {
                        Console.WriteLine($"JWT Middleware: Token validated successfully");
                        // Set the User principal for ASP.NET Core authentication
                        context.User = principal;
                        
                        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        Console.WriteLine($"JWT Middleware: UserId from token: {userId}");
                        if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var userGuid))
                        {
                            var user = await userService.GetByIdAsync(userGuid);
                            if (user != null)
                            {
                                context.Items["User"] = user;
                                context.Items["UserId"] = userGuid;
                                
                                // Ensure the user has the correct role claim
                                var roleClaim = principal.FindFirst(ClaimTypes.Role);
                                if (roleClaim == null)
                                {
                                    // Add role claim if missing
                                    var claims = new List<Claim>(principal.Claims);
                                    if (user.IsAdmin)
                                    {
                                        claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                                    }
                                    else
                                    {
                                        claims.Add(new Claim(ClaimTypes.Role, "User"));
                                    }
                                    
                                    var newPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, principal.Identity?.AuthenticationType));
                                    context.User = newPrincipal;
                                }
                                
                                Console.WriteLine($"JWT Middleware: User found and set in context with role: {context.User.FindFirst(ClaimTypes.Role)?.Value}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"JWT Middleware: Token validation returned null");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"JWT Middleware: Token validation failed: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"JWT Middleware: No token found in Authorization header");
            }

            await _next(context);
        }
    }
} 
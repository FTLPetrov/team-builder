using System.Security.Claims;
using TeamBuilder.Data.Models;

namespace TeamBuilder.Services.Core.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
        string? GetUserIdFromToken(string token);
    }
} 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using TeamBuilder.Data.Models;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Services.Core.Contracts.User.Responses;
using TeamBuilder.Services.Core.Contracts.User.Requests;
using TeamBuilder.Data.Repositories;
using TeamBuilder.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace TeamBuilder.Services.Core
{
public sealed class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly IWarningRepository _warningRepository;
    private readonly HashSet<string> _blacklistedTokens = new();

    public UserService(UserManager<User> userManager, IJwtService jwtService, IWarningRepository warningRepository)
    {
        _userManager = userManager;
        _jwtService = jwtService;
        _warningRepository = warningRepository;
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = _userManager.Users.ToList();
        return users.Select(u => new UserResponse 
        { 
            Id = u.Id, 
            Email = u.Email, 
            UserName = u.UserName,
            FirstName = u.FirstName,
            LastName = u.LastName,
            ProfilePictureUrl = u.ProfilePictureUrl
        });
    }

    public async Task<UserResponse?> GetByIdAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return null;
        return new UserResponse 
        { 
            Id = user.Id, 
            Email = user.Email, 
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl,
            IsAdmin = user.IsAdmin,
            EmailConfirmed = user.EmailConfirmed,
            CreatedAt = DateTime.UtcNow // Since User model doesn't have CreatedAt
        };
    }

    public async Task<UserResponse> CreateAsync(string firstName, string lastName, string email, string username, string password)
    {
        var user = new User(email, username, firstName, lastName);
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        return new UserResponse 
        { 
            Id = user.Id, 
            Email = user.Email, 
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }

    public async Task<UserResponse?> UpdateAsync(Guid id, string firstName, string lastName, string email, string username, string? password = null)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return null;
        

        if (!string.IsNullOrWhiteSpace(firstName))
            user.FirstName = firstName;
        if (!string.IsNullOrWhiteSpace(lastName))
            user.LastName = lastName;
        if (!string.IsNullOrWhiteSpace(email))
            user.Email = email;
        if (!string.IsNullOrWhiteSpace(username))
            user.UserName = username;
        
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
        
        if (!string.IsNullOrWhiteSpace(password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passResult = await _userManager.ResetPasswordAsync(user, token, password);
            if (!passResult.Succeeded)
                throw new Exception(string.Join(", ", passResult.Errors.Select(e => e.Description)));
        }
        
        return new UserResponse 
        { 
            Id = user.Id, 
            Email = user.Email, 
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return false;
        var result = await _userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<UserLoginResponse?> LoginAsync(UserLoginRequest request)
    {
        User? user = null;
        

        if (!string.IsNullOrEmpty(request.Email))
        {
            user = await _userManager.FindByEmailAsync(request.Email);
        }
        
        if (user == null && !string.IsNullOrEmpty(request.Username))
        {
            user = await _userManager.FindByNameAsync(request.Username);
        }
        
        if (user == null) return null;

        var isValidPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isValidPassword) return null;

        var token = _jwtService.GenerateToken(user);
        
        return new UserLoginResponse
        {
            Token = token,
            User = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsAdmin = user.IsAdmin,
                EmailConfirmed = user.EmailConfirmed,
                CreatedAt = DateTime.UtcNow // Since User model doesn't have CreatedAt
            }
        };
    }

    public async Task<bool> LogoutAsync(string token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        

        lock (_blacklistedTokens)
        {
            _blacklistedTokens.Add(token);
        }
        
        return true;
    }

    public async Task<UserResponse?> UpdateProfilePictureAsync(Guid id, IFormFile profilePicture)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return null;


        var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profile-pictures");
        if (!Directory.Exists(uploadsDir))
        {
            Directory.CreateDirectory(uploadsDir);
        }


        var fileExtension = Path.GetExtension(profilePicture.FileName).ToLowerInvariant();
        var fileName = $"{id}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
        var filePath = Path.Combine(uploadsDir, fileName);


        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await profilePicture.CopyToAsync(stream);
        }


        var profilePictureUrl = $"/uploads/profile-pictures/{fileName}";
        user.ProfilePictureUrl = profilePictureUrl;
        
        var result = await _userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            ProfilePictureUrl = user.ProfilePictureUrl
        };
    }

    public async Task<UserResponse?> GetCurrentUserAsync(string token)
    {
        if (string.IsNullOrEmpty(token)) return null;


        lock (_blacklistedTokens)
        {
            if (_blacklistedTokens.Contains(token)) return null;
        }

        var userId = _jwtService.GetUserIdFromToken(token);
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            return null;

        return await GetByIdAsync(userGuid);
    }

    public bool IsTokenBlacklisted(string token)
    {
        lock (_blacklistedTokens)
        {
            return _blacklistedTokens.Contains(token);
        }
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }

    public async Task<IEnumerable<WarningResponse>> GetUserWarningsAsync(Guid userId)
    {
        var warnings = await _warningRepository.GetWarningsByUserAsync(userId);

        return warnings.Select(w => new WarningResponse
        {
            Id = w.Id,
            Message = w.Message,
            UserId = w.UserId,
            UserName = w.User != null ? $"{w.User.FirstName} {w.User.LastName}" : "Unknown User",
            UserEmail = w.User?.Email ?? "Unknown Email",
            CreatedByUserName = w.CreatedByUser != null ? $"{w.CreatedByUser.FirstName} {w.CreatedByUser.LastName}" : "Unknown Admin",
            CreatedAt = w.CreatedAt
        });
    }
}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamBuilder.Data.Models;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Services.Core.Contracts.User.Responses;
using TeamBuilder.Services.Core.Contracts.User.Requests;
using TeamBuilder.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace TeamBuilder.Services.Core
{
public sealed class UserService : IUserService
{
    private readonly UserManager<User> _userManager;
    private readonly IJwtService _jwtService;
    private readonly HashSet<string> _blacklistedTokens = new();

    public UserService(UserManager<User> userManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _jwtService = jwtService;
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
            ProfilePictureUrl = user.ProfilePictureUrl
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
        
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Email = email;
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
        
        // Try to find user by email first, then by username
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
                ProfilePictureUrl = user.ProfilePictureUrl
            }
        };
    }

    public async Task<bool> LogoutAsync(string token)
    {
        if (string.IsNullOrEmpty(token)) return false;
        
        // Add token to blacklist
        lock (_blacklistedTokens)
        {
            _blacklistedTokens.Add(token);
        }
        
        return true;
    }

    public async Task<UserResponse?> UpdateProfilePictureAsync(Guid id, string profilePictureUrl)
    {
        var user = await _userManager.FindByIdAsync(id.ToString());
        if (user == null) return null;

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

        // Check if token is blacklisted
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
}
}

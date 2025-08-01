﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamBuilder.Data.Models;
using TeamBuilder.Services.Core.Contracts.User.Responses;
using TeamBuilder.Services.Core.Contracts.User.Requests;

namespace TeamBuilder.Services.Core.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllAsync();
        Task<UserResponse?> GetByIdAsync(Guid id);
        Task<UserResponse> CreateAsync(string firstName, string lastName, string email, string username, string password);
        Task<UserResponse?> UpdateAsync(Guid id, string firstName, string lastName, string email, string username, string? password = null);
        Task<bool> DeleteAsync(Guid id);
        Task<UserLoginResponse?> LoginAsync(UserLoginRequest request);
        Task<bool> LogoutAsync(string token);
        Task<UserResponse?> UpdateProfilePictureAsync(Guid id, string profilePictureUrl);
        Task<UserResponse?> GetCurrentUserAsync(string token);
    }
}

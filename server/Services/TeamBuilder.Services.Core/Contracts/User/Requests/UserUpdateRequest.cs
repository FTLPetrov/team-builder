using System.ComponentModel.DataAnnotations;
using TeamBuilder.Data.Common;

namespace TeamBuilder.Services.Core.Contracts.User.Requests
{
    public class UserUpdateRequest
    {
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters")]
        [SafeString(ErrorMessage = "First name contains potentially dangerous content")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Last name must be between 2 and 50 characters")]
        [SafeString(ErrorMessage = "Last name contains potentially dangerous content")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email must be less than 100 characters")]
        public string Email { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Username can only contain letters, numbers, underscores, and hyphens")]
        public string UserName { get; set; } = string.Empty;

        [StrongPassword(ErrorMessage = "Password does not meet security requirements")]
        public string? Password { get; set; }
    }
} 
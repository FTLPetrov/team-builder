using Microsoft.AspNetCore.Identity;

namespace TeamBuilder.Data.Models
{
    public sealed class User : IdentityUser<Guid>
    {
        private User() {}

        public User(string email, string username, string firstName, string lastName)
        { 
            Id = Guid.NewGuid();
            Email = email;
            UserName = username;
            FirstName = firstName;
            LastName = lastName;
            EmailConfirmed = false; // Start with unverified email
            IsAdmin = false; // Default to non-admin
            IsDeleted = false; // Default to not deleted
        }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsDeleted { get; set; }
    }
}

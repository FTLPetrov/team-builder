namespace TeamBuilder.Services.Core.Contracts.User.Requests
{
    public class UserLoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
} 
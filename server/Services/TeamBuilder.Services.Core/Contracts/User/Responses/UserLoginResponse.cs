namespace TeamBuilder.Services.Core.Contracts.User.Responses
{
    public class UserLoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserResponse User { get; set; } = new();
    }
} 
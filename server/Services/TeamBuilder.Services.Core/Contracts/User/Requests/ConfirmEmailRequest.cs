namespace TeamBuilder.Services.Core.Contracts.User.Requests
{
    public class ConfirmEmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
} 
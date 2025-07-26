namespace TeamBuilder.Services.Core.Contracts.User.Responses
{
    public class ForgotPasswordResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string Email { get; set; } = string.Empty;
    }
} 
namespace TeamBuilder.Services.Core.Contracts.User.Responses
{
    public class UserCreateResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Guid? Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
    }
} 
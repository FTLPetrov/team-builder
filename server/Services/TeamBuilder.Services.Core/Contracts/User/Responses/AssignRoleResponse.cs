namespace TeamBuilder.Services.Core.Contracts.User.Responses
{
    public class AssignRoleResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
} 
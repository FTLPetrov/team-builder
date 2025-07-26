namespace TeamBuilder.Services.Core.Contracts.User.Requests
{
    public class AssignRoleRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
} 
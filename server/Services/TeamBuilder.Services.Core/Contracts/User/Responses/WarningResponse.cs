namespace TeamBuilder.Services.Core.Contracts.User.Responses
{
    public class WarningResponse
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
} 
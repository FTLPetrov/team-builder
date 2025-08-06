namespace TeamBuilder.Services.Core.Contracts.SupportMessage.Responses
{
    public class SupportMessageResponse
    {
        public Guid Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string UserFirstName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsCompleted { get; set; }
    }
} 
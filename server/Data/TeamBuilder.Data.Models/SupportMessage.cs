namespace TeamBuilder.Data.Models
{
    public class SupportMessage
    {
        public Guid Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public bool IsFavorite { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsDeleted { get; set; }

        private SupportMessage() { }

        public SupportMessage(string subject, string message, Guid userId)
        {
            Id = Guid.NewGuid();
            Subject = subject;
            Message = message;
            UserId = userId;
            CreatedAt = DateTime.UtcNow;
            IsRead = false;
            IsFavorite = false;
            IsCompleted = false;
            IsDeleted = false;
        }
    }
} 
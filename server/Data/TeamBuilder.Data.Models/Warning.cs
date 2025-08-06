namespace TeamBuilder.Data.Models
{
    public class Warning
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; }

        private Warning() { }

        public Warning(string message, Guid userId, Guid createdByUserId)
        {
            Id = Guid.NewGuid();
            Message = message;
            UserId = userId;
            CreatedByUserId = createdByUserId;
            CreatedAt = DateTime.UtcNow;
            IsDeleted = false;
        }
    }
} 
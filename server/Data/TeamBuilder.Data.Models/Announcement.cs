namespace TeamBuilder.Data.Models
{
    public class Announcement
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Guid CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        private Announcement() { }

        public Announcement(string title, string message, Guid createdByUserId)
        {
            Id = Guid.NewGuid();
            Title = title;
            Message = message;
            CreatedByUserId = createdByUserId;
            CreatedAt = DateTime.UtcNow;
            IsActive = true;
            IsDeleted = false;
        }
    }
} 
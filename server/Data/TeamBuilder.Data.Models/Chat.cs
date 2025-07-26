using System;

namespace TeamBuilder.Data.Models
{
    public class Chat
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid SenderId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
        public Team Team { get; set; } = null!;
    }
} 
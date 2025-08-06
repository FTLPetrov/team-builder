using System;

namespace TeamBuilder.Data.Models
{
    public sealed class Chat
    {
        private Chat() {}

        public Chat(Guid teamId, Guid userId, string message)
        {
            Id = Guid.NewGuid();
            TeamId = teamId;
            UserId = userId;
            Message = message;
            CreatedAt = DateTime.UtcNow;
        }

        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid UserId { get; set; }
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }


        public Team Team { get; set; } = null!;
        public User User { get; set; } = null!;
    }
} 
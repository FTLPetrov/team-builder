using System;

namespace TeamBuilder.Data.Models
{
    public class Invitation
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid InvitedUserId { get; set; }
        public Guid InvitedById { get; set; }
        public DateTime SentAt { get; set; }
        public bool Accepted { get; set; }
        public DateTime? RespondedAt { get; set; }
        public Team Team { get; set; } = null!;
    }
} 
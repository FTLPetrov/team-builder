using System;

namespace TeamBuilder.Data.Models
{
    public class EventParticipation
    {
        public Guid Id { get; set; }
        public Guid EventId { get; set; }
        public Guid TeamId { get; set; }
        public DateTime JoinedAt { get; set; }
        public string? Role { get; set; } // Optional role for the team in this event
        
        // Navigation properties
        public Event Event { get; set; } = null!;
        public Team Team { get; set; } = null!;
    }
} 
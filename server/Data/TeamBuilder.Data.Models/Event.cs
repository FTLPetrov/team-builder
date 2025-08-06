using System;

namespace TeamBuilder.Data.Models
{
    public class Event
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string? Location { get; set; }
        public Guid CreatedBy { get; set; }
        public Team Team { get; set; } = null!;
    }
} 
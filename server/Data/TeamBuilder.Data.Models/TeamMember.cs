using System;

namespace TeamBuilder.Data.Models
{
    public class TeamMember
    {
        public Guid UserId { get; set; }
        public Guid TeamId { get; set; }
        public TeamRole Role { get; set; }
        public Team Team { get; set; } = null!;
        public User User { get; set; } = null!;
    }
} 
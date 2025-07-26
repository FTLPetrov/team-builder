using System;
using System.Collections.Generic;

namespace TeamBuilder.Data.Models
{
    public sealed class Team
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
        public Guid OrganizerId { get; set; }
        public ICollection<TeamMember> Members { get; set; } = new List<TeamMember>();
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
} 
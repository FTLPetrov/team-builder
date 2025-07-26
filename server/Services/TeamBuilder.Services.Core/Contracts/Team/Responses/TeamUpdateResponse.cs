using System;
using System.Collections.Generic;

namespace TeamBuilder.Services.Core.Contracts.Team.Responses
{
    public class TeamUpdateResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
        public Guid OrganizerId { get; set; }
        public List<TeamMemberResponse> Members { get; set; } = new();
        public List<EventResponse> Events { get; set; } = new();
    }
} 
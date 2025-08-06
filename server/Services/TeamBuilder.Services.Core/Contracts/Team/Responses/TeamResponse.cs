using System;
using System.Collections.Generic;

namespace TeamBuilder.Services.Core.Contracts.Team.Responses
{
    public class TeamResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsOpen { get; set; }
        public Guid OrganizerId { get; set; }
        public DateTime CreatedAt { get; set; } // For admin service
        public List<TeamMemberResponse> Members { get; set; } = new();
        public List<EventResponse> Events { get; set; } = new();
    }

    public class TeamMemberResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty; // For admin service
        public string LastName { get; set; } = string.Empty; // For admin service
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
    }


} 
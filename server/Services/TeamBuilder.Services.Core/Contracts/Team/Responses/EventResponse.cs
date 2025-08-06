using System;
using TeamBuilder.Services.Core.Contracts.User.Responses;

namespace TeamBuilder.Services.Core.Contracts.Team.Responses
{
    public class EventResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty; // Alias for Name
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime StartDate { get; set; } // Alias for Date
        public DateTime EndDate { get; set; } // For admin service
        public string? Location { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty; // For admin service
        public TeamResponse? Team { get; set; }
        public UserResponse? Organizer { get; set; }
        public List<TeamResponse>? ParticipatingTeams { get; set; }
        public DateTime CreatedAt { get; set; } // For admin service
    }

    public class EventParticipantResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string TeamRole { get; set; } = string.Empty;
        public string? EventRole { get; set; }
    }
} 
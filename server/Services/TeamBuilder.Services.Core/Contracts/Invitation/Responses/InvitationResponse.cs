using System;

namespace TeamBuilder.Services.Core.Contracts.Invitation.Responses
{
    public class InvitationResponse
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public Guid InvitedUserId { get; set; }
        public Guid InvitedById { get; set; }
        public DateTime SentAt { get; set; }
        public bool Accepted { get; set; }
        public DateTime? RespondedAt { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string SenderName { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string InvitedUserName { get; set; } = string.Empty;
        public string InvitedUserEmail { get; set; } = string.Empty;
    }
} 
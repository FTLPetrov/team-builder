using System;

namespace TeamBuilder.Services.Core.Contracts.Invitation.Requests
{
    public class InvitationCreateByEmailRequest
    {
        public Guid TeamId { get; set; }
        public string InvitedUserEmail { get; set; } = string.Empty;
        public Guid InvitedById { get; set; }
    }
} 
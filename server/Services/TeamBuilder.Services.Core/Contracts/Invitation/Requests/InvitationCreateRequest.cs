using System;

namespace TeamBuilder.Services.Core.Contracts.Invitation.Requests
{
    public class InvitationCreateRequest
    {
        public Guid TeamId { get; set; }
        public Guid InvitedUserId { get; set; }
        public Guid InvitedById { get; set; }
    }
} 
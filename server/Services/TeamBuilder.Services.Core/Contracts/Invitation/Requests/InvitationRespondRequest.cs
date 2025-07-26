using System;

namespace TeamBuilder.Services.Core.Contracts.Invitation.Requests
{
    public class InvitationRespondRequest
    {
        public Guid InvitationId { get; set; }
        public bool Accept { get; set; }
    }
} 
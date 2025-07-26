using System;

namespace TeamBuilder.Services.Core.Contracts.Invitation.Responses
{
    public class InvitationRespondResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Guid? Id { get; set; }
        public bool? Accepted { get; set; }
    }
} 
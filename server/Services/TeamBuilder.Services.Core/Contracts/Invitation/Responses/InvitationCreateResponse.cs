using System;

namespace TeamBuilder.Services.Core.Contracts.Invitation.Responses
{
    public class InvitationCreateResponse
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public Guid? Id { get; set; }
    }
} 
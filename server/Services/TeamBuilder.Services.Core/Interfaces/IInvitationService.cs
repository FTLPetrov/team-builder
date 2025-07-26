using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBuilder.Services.Core.Contracts.Invitation.Requests;
using TeamBuilder.Services.Core.Contracts.Invitation.Responses;

namespace TeamBuilder.Services.Core.Interfaces
{
    public interface IInvitationService
    {
        Task<IEnumerable<InvitationResponse>> GetAllAsync(Guid teamId);
        Task<InvitationResponse?> GetByIdAsync(Guid invitationId);
        Task<InvitationCreateResponse> CreateAsync(InvitationCreateRequest request);
        Task<InvitationRespondResponse> RespondAsync(InvitationRespondRequest request);
        Task<bool> DeleteAsync(Guid invitationId);
    }
} 
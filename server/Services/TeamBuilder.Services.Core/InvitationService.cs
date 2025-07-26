using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data;
using TeamBuilder.Data.Models;
using TeamBuilder.Services.Core.Contracts.Invitation.Requests;
using TeamBuilder.Services.Core.Contracts.Invitation.Responses;
using TeamBuilder.Services.Core.Interfaces;

namespace TeamBuilder.Services.Core
{
    public class InvitationService : IInvitationService
    {
        private readonly TeamBuilderDbContext _db;
        public InvitationService(TeamBuilderDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<InvitationResponse>> GetAllAsync(Guid teamId)
        {
            var invitations = await _db.Set<Invitation>().Where(i => i.TeamId == teamId).ToListAsync();
            return invitations.Select(MapToInvitationResponse);
        }

        public async Task<InvitationResponse?> GetByIdAsync(Guid invitationId)
        {
            var invitation = await _db.Set<Invitation>().FindAsync(invitationId);
            return invitation == null ? null : MapToInvitationResponse(invitation);
        }

        public async Task<InvitationCreateResponse> CreateAsync(InvitationCreateRequest request)
        {
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                TeamId = request.TeamId,
                InvitedUserId = request.InvitedUserId,
                InvitedById = request.InvitedById,
                SentAt = DateTime.UtcNow,
                Accepted = false
            };
            _db.Set<Invitation>().Add(invitation);
            await _db.SaveChangesAsync();
            return new InvitationCreateResponse
            {
                Success = true,
                Id = invitation.Id
            };
        }

        public async Task<InvitationRespondResponse> RespondAsync(InvitationRespondRequest request)
        {
            var invitation = await _db.Set<Invitation>().FindAsync(request.InvitationId);
            if (invitation == null)
                return new InvitationRespondResponse { Success = false, ErrorMessage = "Invitation not found" };
            if (invitation.RespondedAt != null)
                return new InvitationRespondResponse { Success = false, ErrorMessage = "Invitation already responded to" };
            invitation.Accepted = request.Accept;
            invitation.RespondedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return new InvitationRespondResponse
            {
                Success = true,
                Id = invitation.Id,
                Accepted = invitation.Accepted
            };
        }

        public async Task<bool> DeleteAsync(Guid invitationId)
        {
            var invitation = await _db.Set<Invitation>().FindAsync(invitationId);
            if (invitation == null) return false;
            _db.Set<Invitation>().Remove(invitation);
            await _db.SaveChangesAsync();
            return true;
        }

        private static InvitationResponse MapToInvitationResponse(Invitation invitation)
        {
            return new InvitationResponse
            {
                Id = invitation.Id,
                TeamId = invitation.TeamId,
                InvitedUserId = invitation.InvitedUserId,
                InvitedById = invitation.InvitedById,
                SentAt = invitation.SentAt,
                Accepted = invitation.Accepted,
                RespondedAt = invitation.RespondedAt
            };
        }
    }
} 
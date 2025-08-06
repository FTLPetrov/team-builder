using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBuilder.Services.Core.Contracts.Team.Requests;
using TeamBuilder.Services.Core.Contracts.Team.Responses;

namespace TeamBuilder.Services.Core.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<TeamResponse>> GetAllAsync();
        Task<IEnumerable<TeamResponse>> GetUserTeamsAsync(Guid userId);
        Task<IEnumerable<TeamResponse>> GetUserOrganizedTeamsAsync(Guid userId);
        Task<TeamResponse?> GetByIdAsync(Guid id);
        Task<TeamCreateResponse> CreateAsync(TeamCreateRequest request);
        Task<TeamUpdateResponse?> UpdateAsync(Guid id, TeamUpdateRequest request);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> InviteMemberAsync(Guid teamId, Guid userId, string role);
        Task<bool> KickMemberAsync(Guid teamId, Guid userId);
        Task<bool> AssignRoleAsync(Guid teamId, Guid userId, string role);
        Task<bool> TransferOwnershipAsync(Guid teamId, Guid newOrganizerId);
        Task<bool> JoinTeamAsync(Guid teamId, Guid userId);
        Task<bool> LeaveTeamAsync(Guid teamId, Guid userId);
        Task<EventResponse?> CreateEventAsync(EventCreateRequest request);
    }
} 
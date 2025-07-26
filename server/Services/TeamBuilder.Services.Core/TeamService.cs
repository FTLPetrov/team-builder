using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data;
using TeamBuilder.Data.Models;
using TeamBuilder.Services.Core.Contracts.Team.Requests;
using TeamBuilder.Services.Core.Contracts.Team.Responses;
using TeamBuilder.Services.Core.Interfaces;

namespace TeamBuilder.Services.Core
{
    public class TeamService : ITeamService
    {
        private readonly TeamBuilderDbContext _db;
        public TeamService(TeamBuilderDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<TeamResponse>> GetAllAsync()
        {
            var teams = await _db.Teams
                .Include(t => t.Members)
                .Include(t => t.Events)
                .ToListAsync();
            return teams.Select(MapToTeamResponse);
        }

        public async Task<TeamResponse?> GetByIdAsync(Guid id)
        {
            var team = await _db.Teams
                .Include(t => t.Members)
                .Include(t => t.Events)
                .FirstOrDefaultAsync(t => t.Id == id);
            return team == null ? null : MapToTeamResponse(team);
        }

        public async Task<TeamCreateResponse> CreateAsync(TeamCreateRequest request)
        {
            var team = new Team
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                IsOpen = request.IsOpen,
                OrganizerId = request.OrganizerId
            };
            team.Members.Add(new TeamMember { UserId = request.OrganizerId, Role = TeamRole.Organizer });
            _db.Teams.Add(team);
            await _db.SaveChangesAsync();
            return new TeamCreateResponse
            {
                Success = true,
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                IsOpen = team.IsOpen,
                OrganizerId = team.OrganizerId,
                Members = team.Members.Select(MapToTeamMemberResponse).ToList(),
                Events = new List<EventResponse>()
            };
        }

        public async Task<TeamUpdateResponse?> UpdateAsync(Guid id, TeamUpdateRequest request)
        {
            var team = await _db.Teams.Include(t => t.Members).Include(t => t.Events).FirstOrDefaultAsync(t => t.Id == id);
            if (team == null) return null;
            team.Name = request.Name;
            team.Description = request.Description;
            team.IsOpen = request.IsOpen;
            await _db.SaveChangesAsync();
            return new TeamUpdateResponse
            {
                Success = true,
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                IsOpen = team.IsOpen,
                OrganizerId = team.OrganizerId,
                Members = team.Members.Select(MapToTeamMemberResponse).ToList(),
                Events = team.Events.Select(MapToEventResponse).ToList()
            };
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var team = await _db.Teams.FindAsync(id);
            if (team == null) return false;
            _db.Teams.Remove(team);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> InviteMemberAsync(Guid teamId, Guid userId, string role)
        {
            var team = await _db.Teams.Include(t => t.Members).FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null) return false;
            if (team.Members.Any(m => m.UserId == userId)) return false;
            if (!Enum.TryParse<TeamRole>(role, out var teamRole)) return false;
            team.Members.Add(new TeamMember { UserId = userId, TeamId = teamId, Role = teamRole });
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> KickMemberAsync(Guid teamId, Guid userId)
        {
            var member = await _db.TeamMembers.FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId);
            if (member == null) return false;
            _db.TeamMembers.Remove(member);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AssignRoleAsync(Guid teamId, Guid userId, string role)
        {
            var member = await _db.TeamMembers.FirstOrDefaultAsync(m => m.TeamId == teamId && m.UserId == userId);
            if (member == null) return false;
            if (!Enum.TryParse<TeamRole>(role, out var teamRole)) return false;
            member.Role = teamRole;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TransferOwnershipAsync(Guid teamId, Guid newOrganizerId)
        {
            var team = await _db.Teams.Include(t => t.Members).FirstOrDefaultAsync(t => t.Id == teamId);
            if (team == null) return false;
            var newOwner = team.Members.FirstOrDefault(m => m.UserId == newOrganizerId);
            if (newOwner == null) return false;
            // Remove old organizer role
            var oldOwner = team.Members.FirstOrDefault(m => m.UserId == team.OrganizerId);
            if (oldOwner != null) oldOwner.Role = TeamRole.Member;
            // Assign new organizer
            newOwner.Role = TeamRole.Organizer;
            team.OrganizerId = newOrganizerId;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<EventResponse?> CreateEventAsync(EventCreateRequest request)
        {
            var team = await _db.Teams.Include(t => t.Events).FirstOrDefaultAsync(t => t.Id == request.TeamId);
            if (team == null) return null;
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                TeamId = request.TeamId,
                Name = request.Name,
                Description = request.Description,
                Date = request.Date,
                CreatedBy = request.CreatedBy
            };
            team.Events.Add(ev);
            await _db.SaveChangesAsync();
            return MapToEventResponse(ev);
        }

        private static TeamResponse MapToTeamResponse(Team team)
        {
            return new TeamResponse
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                IsOpen = team.IsOpen,
                OrganizerId = team.OrganizerId,
                Members = team.Members.Select(MapToTeamMemberResponse).ToList(),
                Events = team.Events.Select(MapToEventResponse).ToList()
            };
        }

        private static TeamMemberResponse MapToTeamMemberResponse(TeamMember member)
        {
            return new TeamMemberResponse
            {
                UserId = member.UserId,
                // UserName and Email would require a join with Users table; left blank for now
                UserName = string.Empty,
                Email = string.Empty,
                Role = member.Role.ToString()
            };
        }

        private static EventResponse MapToEventResponse(Event ev)
        {
            return new EventResponse
            {
                Id = ev.Id,
                Name = ev.Name,
                Description = ev.Description,
                Date = ev.Date,
                CreatedBy = ev.CreatedBy
            };
        }
    }
} 
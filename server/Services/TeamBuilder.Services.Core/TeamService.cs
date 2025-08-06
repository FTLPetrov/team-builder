using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamBuilder.Data.Models;
using TeamBuilder.Services.Core.Contracts.Team.Requests;
using TeamBuilder.Services.Core.Contracts.Team.Responses;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Data.Repositories;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.Services.Core
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IEventRepository _eventRepository;
        
        public TeamService(ITeamRepository teamRepository, ITeamMemberRepository teamMemberRepository, IEventRepository eventRepository)
        {
            _teamRepository = teamRepository;
            _teamMemberRepository = teamMemberRepository;
            _eventRepository = eventRepository;
        }

        public async Task<IEnumerable<TeamResponse>> GetAllAsync()
        {
            var teams = await _teamRepository.GetAllAsync();
            


            return teams.Select(team => MapToTeamResponse(team));
        }

        public async Task<IEnumerable<TeamResponse>> GetUserTeamsAsync(Guid userId)
        {
            var userTeams = await _teamRepository.GetTeamsByMemberAsync(userId);
            return userTeams.Select(team => MapToTeamResponse(team));
        }

        public async Task<IEnumerable<TeamResponse>> GetUserOrganizedTeamsAsync(Guid userId)
        {
            var organizedTeams = await _teamRepository.GetTeamsByOrganizerAsync(userId);
            return organizedTeams.Select(team => MapToTeamResponse(team));
        }

        public async Task<TeamResponse?> GetByIdAsync(Guid id)
        {
            var team = await _teamRepository.GetByIdWithMembersAndEventsAsync(id);
            if (team == null) return null;
            
            return MapToTeamResponse(team);
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
            
            await _teamRepository.AddAsync(team);
            
            var teamMember = new TeamMember { UserId = request.OrganizerId, TeamId = team.Id, Role = TeamRole.Organizer };
            await _teamMemberRepository.AddAsync(teamMember);
            
            return new TeamCreateResponse
            {
                Success = true,
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                IsOpen = team.IsOpen,
                OrganizerId = team.OrganizerId,
                Members = new List<TeamMemberResponse> { MapToTeamMemberResponse(teamMember) },
                Events = new List<EventResponse>()
            };
        }

        public async Task<TeamUpdateResponse?> UpdateAsync(Guid id, TeamUpdateRequest request)
        {
            var team = await _teamRepository.GetByIdWithMembersAndEventsAsync(id);
            if (team == null) return null;
            
            team.Name = request.Name;
            team.Description = request.Description;
            team.IsOpen = request.IsOpen;
            
            await _teamRepository.UpdateAsync(team);
            
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
            var team = await _teamRepository.GetByIdAsync(id);
            if (team == null) return false;
            
            await _teamRepository.DeleteAsync(team);
            return true;
        }

        public async Task<bool> InviteMemberAsync(Guid teamId, Guid userId, string role)
        {
            var team = await _teamRepository.GetByIdWithMembersAsync(teamId);
            if (team == null) return false;
            if (team.Members.Any(m => m.UserId == userId)) return false;
            if (!Enum.TryParse<TeamRole>(role, out var teamRole)) return false;
            
            var teamMember = new TeamMember { UserId = userId, TeamId = teamId, Role = teamRole };
            await _teamMemberRepository.AddAsync(teamMember);
            
            return true;
        }

        public async Task<bool> KickMemberAsync(Guid teamId, Guid userId)
        {
            var member = await _teamMemberRepository.GetByTeamAndUserAsync(teamId, userId);
            if (member == null) return false;
            
            await _teamMemberRepository.DeleteAsync(member);
            return true;
        }

        public async Task<bool> AssignRoleAsync(Guid teamId, Guid userId, string role)
        {
            var member = await _teamMemberRepository.GetByTeamAndUserAsync(teamId, userId);
            if (member == null) return false;
            if (!Enum.TryParse<TeamRole>(role, out var teamRole)) return false;
            
            member.Role = teamRole;
            await _teamMemberRepository.UpdateAsync(member);
            
            return true;
        }

        public async Task<bool> TransferOwnershipAsync(Guid teamId, Guid newOrganizerId)
        {
            var team = await _teamRepository.GetByIdWithMembersAsync(teamId);
            if (team == null) return false;
            
            var newOwner = team.Members.FirstOrDefault(m => m.UserId == newOrganizerId);
            if (newOwner == null) return false;
            

            var oldOwner = team.Members.FirstOrDefault(m => m.UserId == team.OrganizerId);
            if (oldOwner != null)
            {
                oldOwner.Role = TeamRole.Member;
                await _teamMemberRepository.UpdateAsync(oldOwner);
            }
            

            newOwner.Role = TeamRole.Organizer;
            await _teamMemberRepository.UpdateAsync(newOwner);
            
            team.OrganizerId = newOrganizerId;
            await _teamRepository.UpdateAsync(team);
            
            return true;
        }

        public async Task<bool> JoinTeamAsync(Guid teamId, Guid userId)
        {
            var team = await _teamRepository.GetByIdWithMembersAsync(teamId);
            if (team == null) return false;
            

            if (!team.IsOpen) return false;
            

            if (team.Members.Any(m => m.UserId == userId)) return false;
            

            var teamMember = new TeamMember { UserId = userId, TeamId = teamId, Role = TeamRole.Member };
            await _teamMemberRepository.AddAsync(teamMember);
            
            return true;
        }

        public async Task<bool> LeaveTeamAsync(Guid teamId, Guid userId)
        {
            var team = await _teamRepository.GetByIdWithMembersAsync(teamId);
            if (team == null) return false;
            

            var member = team.Members.FirstOrDefault(m => m.UserId == userId);
            if (member == null) return false;
            

            if (team.OrganizerId == userId) return false;
            

            await _teamMemberRepository.DeleteAsync(member);
            
            return true;
        }

        public async Task<EventResponse?> CreateEventAsync(EventCreateRequest request)
        {
            var team = await _teamRepository.GetByIdAsync(request.TeamId);
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
            
            await _eventRepository.AddAsync(ev);
            
            return MapToEventResponse(ev);
        }

        private TeamResponse MapToTeamResponse(Team team)
        {
            return new TeamResponse
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                IsOpen = team.IsOpen,
                OrganizerId = team.OrganizerId,
                Members = team.Members?.Select(MapToTeamMemberResponse).ToList() ?? new List<TeamMemberResponse>(),
                Events = team.Events?.Select(MapToEventResponse).ToList() ?? new List<EventResponse>()
            };
        }

        private static TeamMemberResponse MapToTeamMemberResponse(TeamMember member)
        {
            return new TeamMemberResponse
            {
                UserId = member.UserId,
                UserName = member.User?.UserName ?? "Unknown User",
                FirstName = member.User?.FirstName ?? "Unknown",
                LastName = member.User?.LastName ?? "User",
                Email = member.User?.Email ?? "No email",
                Role = member.Role.ToString(),
                ProfilePictureUrl = member.User?.ProfilePictureUrl
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamBuilder.Data.Models;
using TeamBuilder.Services.Core.Contracts.Team.Requests;
using TeamBuilder.Services.Core.Contracts.Team.Responses;
using TeamBuilder.Services.Core.Contracts.User.Responses;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Data.Repositories;
using TeamBuilder.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TeamBuilder.Services.Core
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IEventParticipationRepository _eventParticipationRepository;
        
        public EventService(IEventRepository eventRepository, ITeamRepository teamRepository, IEventParticipationRepository eventParticipationRepository)
        {
            _eventRepository = eventRepository;
            _teamRepository = teamRepository;
            _eventParticipationRepository = eventParticipationRepository;
        }

        public async Task<IEnumerable<EventResponse>> GetAllAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            var responses = new List<EventResponse>();
            foreach (var ev in events)
            {
                responses.Add(await MapToEventResponseAsync(ev));
            }
            return responses;
        }

        public async Task<IEnumerable<EventResponse>> GetUserEventsAsync(Guid userId)
        {

            var userTeams = await _teamRepository.GetTeamsByMemberAsync(userId);
            var teamIds = userTeams.Select(tm => tm.Id).ToList();

            var events = await _eventRepository.FindAsync(e => teamIds.Contains(e.TeamId));

            var responses = new List<EventResponse>();
            foreach (var ev in events)
            {
                responses.Add(await MapToEventResponseAsync(ev));
            }
            return responses;
        }

        public async Task<IEnumerable<EventResponse>> GetAllAsync(Guid teamId)
        {
            var events = await _eventRepository.GetEventsByTeamAsync(teamId);
            var responses = new List<EventResponse>();
            foreach (var ev in events)
            {
                responses.Add(await MapToEventResponseAsync(ev));
            }
            return responses;
        }

        public async Task<EventResponse?> GetByIdAsync(Guid eventId)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId);
            return ev == null ? null : await MapToEventResponseAsync(ev);
        }

        public async Task<TeamResponse?> GetTeamByIdAsync(Guid teamId)
        {
            var team = await _teamRepository.GetByIdAsync(teamId);
            if (team == null) return null;
            
            return new TeamResponse
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                IsOpen = team.IsOpen,
                OrganizerId = team.OrganizerId
            };
        }

        public async Task<EventCreateResponse> CreateAsync(EventCreateRequest request)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return new EventCreateResponse
                    {
                        Success = false,
                        ErrorMessage = "Event name is required"
                    };
                }

                if (string.IsNullOrWhiteSpace(request.Description))
                {
                    return new EventCreateResponse
                    {
                        Success = false,
                        ErrorMessage = "Event description is required"
                    };
                }

                if (request.Date == default)
                {
                    return new EventCreateResponse
                    {
                        Success = false,
                        ErrorMessage = "Event date is required"
                    };
                }

                var ev = new Event
                {
                    Id = Guid.NewGuid(),
                    TeamId = request.TeamId,
                    Name = request.Name,
                    Description = request.Description,
                    Date = request.Date,
                    Location = request.Location,
                    CreatedBy = request.CreatedBy
                };
                
                await _eventRepository.AddAsync(ev);
                
                return new EventCreateResponse
                {
                    Success = true,
                    Id = ev.Id,
                    Name = ev.Name,
                    Description = ev.Description,
                    Date = ev.Date,
                    CreatedBy = ev.CreatedBy
                };
            }
            catch (Exception ex)
            {
                return new EventCreateResponse
                {
                    Success = false,
                    ErrorMessage = "Failed to create event: " + ex.Message
                };
            }
        }

        public async Task<EventUpdateResponse?> UpdateAsync(Guid eventId, EventUpdateRequest request)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId);
            if (ev == null) return null;
            
            ev.Name = request.Name;
            ev.Description = request.Description;
            ev.Date = request.Date;
            ev.Location = request.Location;
            
            await _eventRepository.UpdateAsync(ev);
            
            return new EventUpdateResponse
            {
                Success = true,
                Id = ev.Id,
                Name = ev.Name,
                Description = ev.Description,
                Date = ev.Date,
                CreatedBy = ev.CreatedBy
            };
        }

        public async Task<bool> DeleteAsync(Guid eventId)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId);
            if (ev == null) return false;
            
            await _eventRepository.DeleteAsync(ev);
            return true;
        }

        private async Task<EventResponse> MapToEventResponseAsync(Event ev)
        {

            var team = await _teamRepository.GetByIdWithMembersAsync(ev.TeamId);


            var participatingTeams = new List<TeamResponse>();
            

            if (team != null)
            {
                participatingTeams.Add(new TeamResponse
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    IsOpen = team.IsOpen,
                    OrganizerId = team.OrganizerId
                });
            }


            var additionalParticipations = await _eventParticipationRepository.GetParticipationsByEventAsync(ev.Id);

            foreach (var participation in additionalParticipations)
            {
                if (participation.Team != null && participation.Team.Id != ev.TeamId) // Don't add host team twice
                {
                    participatingTeams.Add(new TeamResponse
                    {
                        Id = participation.Team.Id,
                        Name = participation.Team.Name,
                        Description = participation.Team.Description,
                        IsOpen = participation.Team.IsOpen,
                        OrganizerId = participation.Team.OrganizerId
                    });
                }
            }

            return new EventResponse
            {
                Id = ev.Id,
                Name = ev.Name,
                Description = ev.Description,
                Date = ev.Date,
                Location = ev.Location,
                CreatedBy = ev.CreatedBy,
                TeamId = ev.TeamId,
                Team = team != null ? new TeamResponse
                {
                    Id = team.Id,
                    Name = team.Name,
                    Description = team.Description,
                    IsOpen = team.IsOpen,
                    OrganizerId = team.OrganizerId
                } : null,
                Organizer = null, // Will be populated separately if needed
                ParticipatingTeams = participatingTeams
            };
        }
    }
} 
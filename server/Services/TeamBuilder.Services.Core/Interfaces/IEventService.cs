using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBuilder.Services.Core.Contracts.Team.Requests;
using TeamBuilder.Services.Core.Contracts.Team.Responses;

namespace TeamBuilder.Services.Core.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<EventResponse>> GetAllAsync();
        Task<IEnumerable<EventResponse>> GetUserEventsAsync(Guid userId);
        Task<IEnumerable<EventResponse>> GetAllAsync(Guid teamId);
        Task<EventResponse?> GetByIdAsync(Guid eventId);
        Task<TeamResponse?> GetTeamByIdAsync(Guid teamId);
        Task<EventCreateResponse> CreateAsync(EventCreateRequest request);
        Task<EventUpdateResponse?> UpdateAsync(Guid eventId, EventUpdateRequest request);
        Task<bool> DeleteAsync(Guid eventId);
    }
} 
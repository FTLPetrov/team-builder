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
    public class EventService : IEventService
    {
        private readonly TeamBuilderDbContext _db;
        public EventService(TeamBuilderDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<EventResponse>> GetAllAsync(Guid teamId)
        {
            var events = await _db.Events.Where(e => e.TeamId == teamId).ToListAsync();
            return events.Select(MapToEventResponse);
        }

        public async Task<EventResponse?> GetByIdAsync(Guid eventId)
        {
            var ev = await _db.Events.FindAsync(eventId);
            return ev == null ? null : MapToEventResponse(ev);
        }

        public async Task<EventCreateResponse> CreateAsync(EventCreateRequest request)
        {
            var ev = new Event
            {
                Id = Guid.NewGuid(),
                TeamId = request.TeamId,
                Name = request.Name,
                Description = request.Description,
                Date = request.Date,
                CreatedBy = request.CreatedBy
            };
            _db.Events.Add(ev);
            await _db.SaveChangesAsync();
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

        public async Task<EventUpdateResponse?> UpdateAsync(Guid eventId, EventUpdateRequest request)
        {
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return null;
            ev.Name = request.Name;
            ev.Description = request.Description;
            ev.Date = request.Date;
            await _db.SaveChangesAsync();
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
            var ev = await _db.Events.FindAsync(eventId);
            if (ev == null) return false;
            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();
            return true;
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
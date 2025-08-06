using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.Data.Repositories
{
    public class EventParticipationRepository : Repository<EventParticipation>, IEventParticipationRepository
    {
        public EventParticipationRepository(TeamBuilderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<EventParticipation>> GetParticipationsByEventAsync(Guid eventId)
        {
            return await _dbSet
                .Include(ep => ep.Team)
                .Where(ep => ep.EventId == eventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<EventParticipation>> GetParticipationsByTeamAsync(Guid teamId)
        {
            return await _dbSet
                .Include(ep => ep.Event)
                .Where(ep => ep.TeamId == teamId)
                .ToListAsync();
        }

        public async Task<bool> IsTeamParticipatingInEventAsync(Guid teamId, Guid eventId)
        {
            return await _dbSet
                .AnyAsync(ep => ep.TeamId == teamId && ep.EventId == eventId);
        }
    }
}

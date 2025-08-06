using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.Data.Repositories
{
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(TeamBuilderDbContext context) : base(context)
        {
        }

        public async Task<Event?> GetByIdWithTeamAsync(Guid id)
        {
            return await _dbSet
                .Include(e => e.Team)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Event>> GetEventsByTeamAsync(Guid teamId)
        {
            return await _dbSet
                .Include(e => e.Team)
                .Where(e => e.TeamId == teamId)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByCreatorAsync(Guid creatorId)
        {
            return await _dbSet
                .Include(e => e.Team)
                .Where(e => e.CreatedBy == creatorId)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync()
        {
            return await _dbSet
                .Include(e => e.Team)
                .Where(e => e.Date >= DateTime.UtcNow)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(e => e.Team)
                .Where(e => e.Date >= startDate && e.Date <= endDate)
                .OrderBy(e => e.Date)
                .ToListAsync();
        }
    }
}

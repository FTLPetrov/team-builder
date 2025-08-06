using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.Data.Repositories
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        public TeamRepository(TeamBuilderDbContext context) : base(context)
        {
        }

        public async Task<Team?> GetByIdWithMembersAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Team?> GetByIdWithMembersAndEventsAsync(Guid id)
        {
            return await _dbSet
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .Include(t => t.Events)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Team>> GetTeamsByOrganizerAsync(Guid organizerId)
        {
            return await _dbSet
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .Where(t => t.OrganizerId == organizerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetOpenTeamsAsync()
        {
            return await _dbSet
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .Where(t => t.IsOpen)
                .ToListAsync();
        }

        public async Task<IEnumerable<Team>> GetTeamsByMemberAsync(Guid memberId)
        {
            return await _dbSet
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .Where(t => t.Members.Any(m => m.UserId == memberId))
                .ToListAsync();
        }

        public async Task<bool> IsUserMemberOfTeamAsync(Guid teamId, Guid userId)
        {
            return await _context.TeamMembers
                .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
        }

        public async Task<bool> IsUserOrganizerOfTeamAsync(Guid teamId, Guid userId)
        {
            return await _dbSet
                .AnyAsync(t => t.Id == teamId && t.OrganizerId == userId);
        }

        public override async Task<IEnumerable<Team>> GetAllAsync()
        {
            return await _dbSet
                .Include(t => t.Members)
                    .ThenInclude(m => m.User)
                .ToListAsync();
        }
    }
}

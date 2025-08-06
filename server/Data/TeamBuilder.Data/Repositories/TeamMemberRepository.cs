using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.Data.Repositories
{
    public class TeamMemberRepository : Repository<TeamMember>, ITeamMemberRepository
    {
        public TeamMemberRepository(TeamBuilderDbContext context) : base(context)
        {
        }

        public async Task<TeamMember?> GetByTeamAndUserAsync(Guid teamId, Guid userId)
        {
            return await _dbSet
                .Include(tm => tm.User)
                .Include(tm => tm.Team)
                .FirstOrDefaultAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
        }

        public async Task<IEnumerable<TeamMember>> GetMembersByTeamAsync(Guid teamId)
        {
            return await _dbSet
                .Include(tm => tm.User)
                .Where(tm => tm.TeamId == teamId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TeamMember>> GetTeamsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Include(tm => tm.Team)
                .Where(tm => tm.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> IsUserMemberOfTeamAsync(Guid teamId, Guid userId)
        {
            return await _dbSet
                .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId);
        }

        public async Task<bool> IsUserOrganizerOfTeamAsync(Guid teamId, Guid userId)
        {
            return await _dbSet
                .AnyAsync(tm => tm.TeamId == teamId && tm.UserId == userId && tm.Role == TeamRole.Organizer);
        }
    }
}

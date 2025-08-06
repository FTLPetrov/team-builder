using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.Data.Repositories
{
    public class InvitationRepository : Repository<Invitation>, IInvitationRepository
    {
        public InvitationRepository(TeamBuilderDbContext context) : base(context)
        {
        }

        public async Task<Invitation?> GetByIdWithTeamAsync(Guid id)
        {
            return await _dbSet
                .Include(i => i.Team)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<IEnumerable<Invitation>> GetInvitationsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Include(i => i.Team)
                .Where(i => i.InvitedUserId == userId)
                .OrderByDescending(i => i.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invitation>> GetInvitationsByTeamAsync(Guid teamId)
        {
            return await _dbSet
                .Include(i => i.Team)
                .Where(i => i.TeamId == teamId)
                .OrderByDescending(i => i.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invitation>> GetPendingInvitationsAsync(Guid userId)
        {
            return await _dbSet
                .Include(i => i.Team)
                .Where(i => i.InvitedUserId == userId && i.RespondedAt == null)
                .OrderByDescending(i => i.SentAt)
                .ToListAsync();
        }

        public async Task<bool> HasPendingInvitationAsync(Guid teamId, Guid userId)
        {
            return await _dbSet
                .AnyAsync(i => i.TeamId == teamId && i.InvitedUserId == userId && i.RespondedAt == null);
        }
    }
}

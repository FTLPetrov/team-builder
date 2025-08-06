using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.Data.Repositories
{
    public class ChatRepository : Repository<Chat>, IChatRepository
    {
        public ChatRepository(TeamBuilderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Chat>> GetMessagesByTeamAsync(Guid teamId, int page = 1, int pageSize = 20)
        {
            return await _dbSet
                .Include(c => c.User)
                .Where(c => c.TeamId == teamId)
                .OrderBy(c => c.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<IEnumerable<Chat>> GetAllMessagesByTeamAsync(Guid teamId)
        {
            return await _dbSet
                .Include(c => c.User)
                .Where(c => c.TeamId == teamId)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetMessageCountByTeamAsync(Guid teamId)
        {
            return await _dbSet
                .CountAsync(c => c.TeamId == teamId);
        }
    }
}

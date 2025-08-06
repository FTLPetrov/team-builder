using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.Data.Repositories
{
    public class SupportMessageRepository : Repository<SupportMessage>, ISupportMessageRepository
    {
        public SupportMessageRepository(TeamBuilderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SupportMessage>> GetMessagesByUserAsync(Guid userId)
        {
            return await _dbSet
                .Where(sm => sm.UserId == userId && !sm.IsDeleted)
                .OrderByDescending(sm => sm.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SupportMessage>> GetActiveMessagesAsync()
        {
            return await _dbSet
                .Where(sm => !sm.IsCompleted && !sm.IsDeleted)
                .OrderByDescending(sm => sm.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SupportMessage>> GetCompletedMessagesAsync()
        {
            return await _dbSet
                .Where(sm => sm.IsCompleted && !sm.IsDeleted)
                .OrderByDescending(sm => sm.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SupportMessage>> GetMessagesByStatusAsync(bool isCompleted)
        {
            return await _dbSet
                .Where(sm => sm.IsCompleted == isCompleted && !sm.IsDeleted)
                .OrderByDescending(sm => sm.CreatedAt)
                .ToListAsync();
        }
    }
}

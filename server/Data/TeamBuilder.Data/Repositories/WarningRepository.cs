using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.Data.Repositories
{
    public class WarningRepository : Repository<Warning>, IWarningRepository
    {
        public WarningRepository(TeamBuilderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Warning>> GetWarningsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Include(w => w.User)
                .Include(w => w.CreatedByUser)
                .Where(w => w.UserId == userId && !w.IsDeleted)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Warning>> GetWarningsByAdminAsync(Guid adminId)
        {
            return await _dbSet
                .Include(w => w.User)
                .Include(w => w.CreatedByUser)
                .Where(w => w.CreatedByUserId == adminId && !w.IsDeleted)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Warning>> GetActiveWarningsAsync()
        {
            return await _dbSet
                .Include(w => w.User)
                .Include(w => w.CreatedByUser)
                .Where(w => !w.IsDeleted)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }
    }
}

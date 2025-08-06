using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.Data.Repositories
{
    public class AnnouncementRepository : Repository<Announcement>, IAnnouncementRepository
    {
        public AnnouncementRepository(TeamBuilderDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Announcement>> GetActiveAnnouncementsAsync()
        {
            return await _dbSet
                .Include(a => a.CreatedByUser)
                .Where(a => a.IsActive && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Announcement>> GetAllAnnouncementsAsync()
        {
            return await _dbSet
                .Include(a => a.CreatedByUser)
                .Where(a => !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Announcement>> GetAnnouncementsByCreatorAsync(Guid creatorId)
        {
            return await _dbSet
                .Include(a => a.CreatedByUser)
                .Where(a => a.CreatedByUserId == creatorId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Announcement>> GetAnnouncementsByStatusAsync(bool isActive)
        {
            return await _dbSet
                .Include(a => a.CreatedByUser)
                .Where(a => a.IsActive == isActive && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }
    }
}

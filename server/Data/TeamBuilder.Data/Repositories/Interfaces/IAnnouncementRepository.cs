using TeamBuilder.Data.Models;

namespace TeamBuilder.Data.Repositories.Interfaces
{
    public interface IAnnouncementRepository : IRepository<Announcement>
    {
        Task<IEnumerable<Announcement>> GetActiveAnnouncementsAsync();
        Task<IEnumerable<Announcement>> GetAllAnnouncementsAsync();
        Task<IEnumerable<Announcement>> GetAnnouncementsByCreatorAsync(Guid creatorId);
        Task<IEnumerable<Announcement>> GetAnnouncementsByStatusAsync(bool isActive);
    }
}

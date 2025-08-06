using TeamBuilder.Data.Models;

namespace TeamBuilder.Data.Repositories.Interfaces
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<Event?> GetByIdWithTeamAsync(Guid id);
        Task<IEnumerable<Event>> GetEventsByTeamAsync(Guid teamId);
        Task<IEnumerable<Event>> GetEventsByCreatorAsync(Guid creatorId);
        Task<IEnumerable<Event>> GetUpcomingEventsAsync();
        Task<IEnumerable<Event>> GetEventsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}

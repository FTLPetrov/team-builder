using TeamBuilder.Data.Models;

namespace TeamBuilder.Data.Repositories.Interfaces
{
    public interface IEventParticipationRepository : IRepository<EventParticipation>
    {
        Task<IEnumerable<EventParticipation>> GetParticipationsByEventAsync(Guid eventId);
        Task<IEnumerable<EventParticipation>> GetParticipationsByTeamAsync(Guid teamId);
        Task<bool> IsTeamParticipatingInEventAsync(Guid teamId, Guid eventId);
    }
}

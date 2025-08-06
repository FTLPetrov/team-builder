using TeamBuilder.Data.Models;

namespace TeamBuilder.Data.Repositories.Interfaces
{
    public interface ITeamRepository : IRepository<Team>
    {
        Task<Team?> GetByIdWithMembersAsync(Guid id);
        Task<Team?> GetByIdWithMembersAndEventsAsync(Guid id);
        Task<IEnumerable<Team>> GetTeamsByOrganizerAsync(Guid organizerId);
        Task<IEnumerable<Team>> GetOpenTeamsAsync();
        Task<IEnumerable<Team>> GetTeamsByMemberAsync(Guid memberId);
        Task<bool> IsUserMemberOfTeamAsync(Guid teamId, Guid userId);
        Task<bool> IsUserOrganizerOfTeamAsync(Guid teamId, Guid userId);
    }
}

using TeamBuilder.Data.Models;

namespace TeamBuilder.Data.Repositories.Interfaces
{
    public interface ITeamMemberRepository : IRepository<TeamMember>
    {
        Task<TeamMember?> GetByTeamAndUserAsync(Guid teamId, Guid userId);
        Task<IEnumerable<TeamMember>> GetMembersByTeamAsync(Guid teamId);
        Task<IEnumerable<TeamMember>> GetTeamsByUserAsync(Guid userId);
        Task<bool> IsUserMemberOfTeamAsync(Guid teamId, Guid userId);
        Task<bool> IsUserOrganizerOfTeamAsync(Guid teamId, Guid userId);
    }
}

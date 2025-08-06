using TeamBuilder.Data.Models;

namespace TeamBuilder.Data.Repositories.Interfaces
{
    public interface IInvitationRepository : IRepository<Invitation>
    {
        Task<Invitation?> GetByIdWithTeamAsync(Guid id);
        Task<IEnumerable<Invitation>> GetInvitationsByUserAsync(Guid userId);
        Task<IEnumerable<Invitation>> GetInvitationsByTeamAsync(Guid teamId);
        Task<IEnumerable<Invitation>> GetPendingInvitationsAsync(Guid userId);
        Task<bool> HasPendingInvitationAsync(Guid teamId, Guid userId);
    }
}

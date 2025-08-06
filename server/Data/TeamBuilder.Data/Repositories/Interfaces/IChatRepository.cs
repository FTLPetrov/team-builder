using TeamBuilder.Data.Models;

namespace TeamBuilder.Data.Repositories.Interfaces
{
    public interface IChatRepository : IRepository<Chat>
    {
        Task<IEnumerable<Chat>> GetMessagesByTeamAsync(Guid teamId, int page = 1, int pageSize = 20);
        Task<IEnumerable<Chat>> GetAllMessagesByTeamAsync(Guid teamId);
        Task<int> GetMessageCountByTeamAsync(Guid teamId);
    }
}

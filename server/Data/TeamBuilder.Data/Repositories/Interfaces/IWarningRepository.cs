using TeamBuilder.Data.Models;

namespace TeamBuilder.Data.Repositories.Interfaces
{
    public interface IWarningRepository : IRepository<Warning>
    {
        Task<IEnumerable<Warning>> GetWarningsByUserAsync(Guid userId);
        Task<IEnumerable<Warning>> GetWarningsByAdminAsync(Guid adminId);
        Task<IEnumerable<Warning>> GetActiveWarningsAsync();
    }
}

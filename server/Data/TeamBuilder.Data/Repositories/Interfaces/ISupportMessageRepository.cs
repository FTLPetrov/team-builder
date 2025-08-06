using TeamBuilder.Data.Models;

namespace TeamBuilder.Data.Repositories.Interfaces
{
    public interface ISupportMessageRepository : IRepository<SupportMessage>
    {
        Task<IEnumerable<SupportMessage>> GetMessagesByUserAsync(Guid userId);
        Task<IEnumerable<SupportMessage>> GetActiveMessagesAsync();
        Task<IEnumerable<SupportMessage>> GetCompletedMessagesAsync();
        Task<IEnumerable<SupportMessage>> GetMessagesByStatusAsync(bool isCompleted);
    }
}

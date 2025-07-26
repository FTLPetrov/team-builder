using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBuilder.Services.Core.Contracts.Chat.Requests;
using TeamBuilder.Services.Core.Contracts.Chat.Responses;

namespace TeamBuilder.Services.Core.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<ChatResponse>> GetAllAsync(Guid teamId);
        Task<ChatResponse?> GetByIdAsync(Guid chatId);
        Task<ChatCreateResponse> CreateAsync(ChatCreateRequest request);
        Task<bool> DeleteAsync(Guid chatId);
    }
} 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamBuilder.Services.Core.Contracts.Chat.Requests;
using TeamBuilder.Services.Core.Contracts.Chat.Responses;

namespace TeamBuilder.Services.Core.Interfaces
{
    public interface IChatService
    {
        Task<IEnumerable<ChatResponse>> GetTeamMessagesAsync(Guid teamId, int page = 1, int pageSize = 20);
        Task<ChatResponse> CreateMessageAsync(ChatCreateRequest request, Guid userId);
    }
} 
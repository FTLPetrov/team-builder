using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamBuilder.Data.Models;
using TeamBuilder.Services.Core.Contracts.Chat.Requests;
using TeamBuilder.Services.Core.Contracts.Chat.Responses;
using TeamBuilder.Services.Core.Interfaces;
using TeamBuilder.Data.Repositories;
using TeamBuilder.Data.Repositories.Interfaces;

namespace TeamBuilder.Services.Core
{
    public class ChatService : IChatService
    {
        private readonly IChatRepository _chatRepository;
        
        public ChatService(IChatRepository chatRepository)
        {
            _chatRepository = chatRepository;
        }

        public async Task<IEnumerable<ChatResponse>> GetTeamMessagesAsync(Guid teamId, int page = 1, int pageSize = 20)
        {
            var chats = await _chatRepository.GetMessagesByTeamAsync(teamId, page, pageSize);
            return chats.Select(MapToChatResponse);
        }

        public async Task<ChatResponse> CreateMessageAsync(ChatCreateRequest request, Guid userId)
        {
            var chat = new Chat(request.TeamId, userId, request.Message);
            
            await _chatRepository.AddAsync(chat);
            

            var reloadedChat = await _chatRepository.GetByIdAsync(chat.Id);
            return MapToChatResponse(reloadedChat);
        }

        public async Task<IEnumerable<ChatResponse>> GetAllAsync(Guid teamId)
        {
            var chats = await _chatRepository.GetAllMessagesByTeamAsync(teamId);
            return chats.Select(MapToChatResponse);
        }

        public async Task<ChatResponse?> GetByIdAsync(Guid chatId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            return chat == null ? null : MapToChatResponse(chat);
        }

        public async Task<ChatCreateResponse> CreateAsync(ChatCreateRequest request)
        {

            throw new NotImplementedException("Use CreateMessageAsync with userId parameter instead");
        }

        public async Task<bool> DeleteAsync(Guid chatId)
        {
            var chat = await _chatRepository.GetByIdAsync(chatId);
            if (chat == null) return false;
            
            await _chatRepository.DeleteAsync(chat);
            return true;
        }

        private static ChatResponse MapToChatResponse(Chat chat)
        {
            return new ChatResponse
            {
                Id = chat.Id,
                TeamId = chat.TeamId,
                UserId = chat.UserId,
                UserName = chat.User?.UserName ?? string.Empty,
                UserFirstName = chat.User?.FirstName ?? string.Empty,
                UserLastName = chat.User?.LastName ?? string.Empty,
                Message = chat.Message,
                CreatedAt = chat.CreatedAt
            };
        }
    }
} 
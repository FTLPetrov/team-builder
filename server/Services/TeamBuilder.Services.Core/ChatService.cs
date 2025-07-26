using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TeamBuilder.Data;
using TeamBuilder.Data.Models;
using TeamBuilder.Services.Core.Contracts.Chat.Requests;
using TeamBuilder.Services.Core.Contracts.Chat.Responses;
using TeamBuilder.Services.Core.Interfaces;

namespace TeamBuilder.Services.Core
{
    public class ChatService : IChatService
    {
        private readonly TeamBuilderDbContext _db;
        public ChatService(TeamBuilderDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<ChatResponse>> GetAllAsync(Guid teamId)
        {
            var chats = await _db.Set<Chat>().Where(c => c.TeamId == teamId).OrderBy(c => c.SentAt).ToListAsync();
            return chats.Select(MapToChatResponse);
        }

        public async Task<ChatResponse?> GetByIdAsync(Guid chatId)
        {
            var chat = await _db.Set<Chat>().FindAsync(chatId);
            return chat == null ? null : MapToChatResponse(chat);
        }

        public async Task<ChatCreateResponse> CreateAsync(ChatCreateRequest request)
        {
            var chat = new Chat
            {
                Id = Guid.NewGuid(),
                TeamId = request.TeamId,
                SenderId = request.SenderId,
                Message = request.Message,
                SentAt = DateTime.UtcNow
            };
            _db.Set<Chat>().Add(chat);
            await _db.SaveChangesAsync();
            return new ChatCreateResponse
            {
                Success = true,
                Id = chat.Id
            };
        }

        public async Task<bool> DeleteAsync(Guid chatId)
        {
            var chat = await _db.Set<Chat>().FindAsync(chatId);
            if (chat == null) return false;
            _db.Set<Chat>().Remove(chat);
            await _db.SaveChangesAsync();
            return true;
        }

        private static ChatResponse MapToChatResponse(Chat chat)
        {
            return new ChatResponse
            {
                Id = chat.Id,
                TeamId = chat.TeamId,
                SenderId = chat.SenderId,
                Message = chat.Message,
                SentAt = chat.SentAt
            };
        }
    }
} 
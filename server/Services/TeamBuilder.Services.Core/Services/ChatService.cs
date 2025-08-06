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

namespace TeamBuilder.Services.Core.Services
{
    public sealed class ChatService : IChatService
    {
        private readonly TeamBuilderDbContext _context;

        public ChatService(TeamBuilderDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ChatResponse>> GetTeamMessagesAsync(Guid teamId, int page = 1, int pageSize = 20)
        {
            var messages = await _context.Chats
                .Include(c => c.User)
                .Where(c => c.TeamId == teamId)
                .OrderBy(c => c.CreatedAt) // Oldest first
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ChatResponse
                {
                    Id = c.Id,
                    TeamId = c.TeamId,
                    UserId = c.UserId,
                    UserName = c.User.UserName,
                    UserFirstName = c.User.FirstName,
                    UserLastName = c.User.LastName,
                    Message = c.Message,
                    CreatedAt = c.CreatedAt
                })
                .ToListAsync();

            return messages;
        }

        public async Task<ChatResponse> CreateMessageAsync(ChatCreateRequest request, Guid userId)
        {
            var chat = new Chat(request.TeamId, userId, request.Message);
            
            _context.Chats.Add(chat);
            await _context.SaveChangesAsync();

            // Reload with user data
            await _context.Entry(chat).Reference(c => c.User).LoadAsync();

            return new ChatResponse
            {
                Id = chat.Id,
                TeamId = chat.TeamId,
                UserId = chat.UserId,
                UserName = chat.User.UserName,
                UserFirstName = chat.User.FirstName,
                UserLastName = chat.User.LastName,
                Message = chat.Message,
                CreatedAt = chat.CreatedAt
            };
        }
    }
} 
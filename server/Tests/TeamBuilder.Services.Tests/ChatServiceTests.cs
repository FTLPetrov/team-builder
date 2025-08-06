using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;
using TeamBuilder.Services.Core;
using TeamBuilder.Services.Core.Contracts.Chat.Requests;
using TeamBuilder.Services.Core.Contracts.Chat.Responses;

namespace TeamBuilder.Services.Tests
{
    [TestFixture]
    public class ChatServiceTests
    {
        private Mock<IChatRepository> _chatRepositoryMock = null!;
        private ChatService _chatService = null!;

        [SetUp]
        public void Setup()
        {
            _chatRepositoryMock = new Mock<IChatRepository>();
            _chatService = new ChatService(_chatRepositoryMock.Object);
        }

        [Test]
        public async Task GetTeamMessagesAsync_WithValidTeamId_ShouldReturnMessages()
        {

            var teamId = Guid.NewGuid();
            var chats = new List<Chat>
            {
                new Chat(teamId, Guid.NewGuid(), "Message 1"),
                new Chat(teamId, Guid.NewGuid(), "Message 2"),
                new Chat(teamId, Guid.NewGuid(), "Message 3")
            };

            _chatRepositoryMock.Setup(x => x.GetMessagesByTeamAsync(teamId, 1, 20))
                .ReturnsAsync(chats);


            var result = await _chatService.GetTeamMessagesAsync(teamId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetTeamMessagesAsync_WithCustomPagination_ShouldReturnMessages()
        {

            var teamId = Guid.NewGuid();
            var chats = new List<Chat>
            {
                new Chat(teamId, Guid.NewGuid(), "Message 1"),
                new Chat(teamId, Guid.NewGuid(), "Message 2")
            };

            _chatRepositoryMock.Setup(x => x.GetMessagesByTeamAsync(teamId, 1, 2))
                .ReturnsAsync(chats);


            var result = await _chatService.GetTeamMessagesAsync(teamId, 1, 2);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task CreateMessageAsync_WithValidRequest_ShouldCreateMessage()
        {

            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new ChatCreateRequest
            {
                TeamId = teamId,
                Message = "Test Message"
            };

            var chat = new Chat(teamId, userId, "Test Message")
            {
                User = new User("test@test.com", "testuser", "John", "Doe") { Id = userId }
            };

            _chatRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Chat>()))
                .ReturnsAsync(chat);
            _chatRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(chat);


            var result = await _chatService.CreateMessageAsync(request, userId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.TeamId, Is.EqualTo(teamId));
            Assert.That(result.UserId, Is.EqualTo(userId));
            Assert.That(result.Message, Is.EqualTo("Test Message"));
        }

        [Test]
        public async Task GetAllAsync_WithValidTeamId_ShouldReturnAllMessages()
        {

            var teamId = Guid.NewGuid();
            var chats = new List<Chat>
            {
                new Chat(teamId, Guid.NewGuid(), "Message 1"),
                new Chat(teamId, Guid.NewGuid(), "Message 2"),
                new Chat(teamId, Guid.NewGuid(), "Message 3")
            };

            _chatRepositoryMock.Setup(x => x.GetAllMessagesByTeamAsync(teamId))
                .ReturnsAsync(chats);


            var result = await _chatService.GetAllAsync(teamId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(3));
        }

        [Test]
        public async Task GetByIdAsync_WithValidId_ShouldReturnChat()
        {

            var chatId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var chat = new Chat(teamId, userId, "Test Message") { Id = chatId };

            _chatRepositoryMock.Setup(x => x.GetByIdAsync(chatId))
                .ReturnsAsync(chat);


            var result = await _chatService.GetByIdAsync(chatId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(chatId));
            Assert.That(result.TeamId, Is.EqualTo(teamId));
            Assert.That(result.UserId, Is.EqualTo(userId));
            Assert.That(result.Message, Is.EqualTo("Test Message"));
        }

        [Test]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {

            var chatId = Guid.NewGuid();
            _chatRepositoryMock.Setup(x => x.GetByIdAsync(chatId))
                .ReturnsAsync((Chat?)null);


            var result = await _chatService.GetByIdAsync(chatId);


            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WithValidId_ShouldDeleteChat()
        {

            var chatId = Guid.NewGuid();
            var chat = new Chat(Guid.NewGuid(), Guid.NewGuid(), "Test Message") { Id = chatId };

            _chatRepositoryMock.Setup(x => x.GetByIdAsync(chatId))
                .ReturnsAsync(chat);
            _chatRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Chat>()))
                .Returns(Task.CompletedTask);


            var result = await _chatService.DeleteAsync(chatId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {

            var chatId = Guid.NewGuid();
            _chatRepositoryMock.Setup(x => x.GetByIdAsync(chatId))
                .ReturnsAsync((Chat?)null);


            var result = await _chatService.DeleteAsync(chatId);


            Assert.That(result, Is.False);
        }

        [Test]
        public void CreateAsync_ShouldThrowNotImplementedException()
        {
 & Assert
            var ex = Assert.ThrowsAsync<NotImplementedException>(async () =>
                await _chatService.CreateAsync(new ChatCreateRequest { TeamId = Guid.NewGuid(), Message = "Test" }));

            Assert.That(ex.Message, Does.Contain("CreateMessageAsync with userId parameter"));
        }

        [Test]
        public async Task CreateMessageAsync_WithEmptyMessage_ShouldCreateMessage()
        {

            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new ChatCreateRequest
            {
                TeamId = teamId,
                Message = ""
            };

            var chat = new Chat(teamId, userId, "") 
            {
                User = new User("test@test.com", "testuser", "John", "Doe") { Id = userId }
            };

            _chatRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Chat>()))
                .ReturnsAsync(chat);
            _chatRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(chat);


            var result = await _chatService.CreateMessageAsync(request, userId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Message, Is.EqualTo(""));
        }

        [Test]
        public async Task GetTeamMessagesAsync_WithEmptyResult_ShouldReturnEmptyList()
        {

            var teamId = Guid.NewGuid();
            var chats = new List<Chat>();

            _chatRepositoryMock.Setup(x => x.GetMessagesByTeamAsync(teamId, 1, 20))
                .ReturnsAsync(chats);


            var result = await _chatService.GetTeamMessagesAsync(teamId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllAsync_WithEmptyResult_ShouldReturnEmptyList()
        {

            var teamId = Guid.NewGuid();
            var chats = new List<Chat>();

            _chatRepositoryMock.Setup(x => x.GetAllMessagesByTeamAsync(teamId))
                .ReturnsAsync(chats);


            var result = await _chatService.GetAllAsync(teamId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }
    }
}

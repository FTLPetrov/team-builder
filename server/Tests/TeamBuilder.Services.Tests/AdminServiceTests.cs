using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;
using TeamBuilder.Services.Core;
using TeamBuilder.Services.Core.Contracts.Announcement.Requests;
using TeamBuilder.Services.Core.Contracts.Announcement.Responses;
using TeamBuilder.Services.Core.Contracts.SupportMessage.Responses;
using TeamBuilder.Services.Core.Contracts.Team.Responses;
using TeamBuilder.Services.Core.Contracts.User.Requests;
using TeamBuilder.Services.Core.Contracts.User.Responses;

namespace TeamBuilder.Services.Tests
{
    [TestFixture]
    public class AdminServiceTests
    {
        private Mock<ITeamRepository> _teamRepositoryMock = null!;
        private Mock<ITeamMemberRepository> _teamMemberRepositoryMock = null!;
        private Mock<IEventRepository> _eventRepositoryMock = null!;
        private Mock<IChatRepository> _chatRepositoryMock = null!;
        private Mock<ISupportMessageRepository> _supportMessageRepositoryMock = null!;
        private Mock<IAnnouncementRepository> _announcementRepositoryMock = null!;
        private Mock<IWarningRepository> _warningRepositoryMock = null!;
        private Mock<UserManager<User>> _userManagerMock = null!;
        private AdminService _adminService = null!;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _teamRepositoryMock = new Mock<ITeamRepository>();
            _teamMemberRepositoryMock = new Mock<ITeamMemberRepository>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _chatRepositoryMock = new Mock<IChatRepository>();
            _supportMessageRepositoryMock = new Mock<ISupportMessageRepository>();
            _announcementRepositoryMock = new Mock<IAnnouncementRepository>();
            _warningRepositoryMock = new Mock<IWarningRepository>();
            _adminService = new AdminService(
                _teamRepositoryMock.Object,
                _teamMemberRepositoryMock.Object,
                _eventRepositoryMock.Object,
                _chatRepositoryMock.Object,
                _supportMessageRepositoryMock.Object,
                _announcementRepositoryMock.Object,
                _warningRepositoryMock.Object,
                _userManagerMock.Object);
        }

        [Test]
        public async Task GetAllUsersAsync_ShouldReturnAllUsers()
        {

            var users = new List<User>
            {
                new User("user1@test.com", "user1", "John", "Doe") { Id = Guid.NewGuid(), IsAdmin = false },
                new User("user2@test.com", "user2", "Jane", "Smith") { Id = Guid.NewGuid(), IsAdmin = true }
            };

            _userManagerMock.Setup(x => x.Users).Returns(users.AsQueryable());


            var result = await _adminService.GetAllUsersAsync();


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Email, Is.EqualTo("user1@test.com"));
        }

        [Test]
        public async Task GetUserByIdAsync_WithValidId_ShouldReturnUser()
        {

            var userId = Guid.NewGuid();
            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = userId, IsAdmin = true };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);


            var result = await _adminService.GetUserByIdAsync(userId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(userId));
            Assert.That(result.Email, Is.EqualTo("test@test.com"));
            Assert.That(result.IsAdmin, Is.True);
        }

        [Test]
        public async Task GetUserByIdAsync_WithInvalidId_ShouldReturnNull()
        {

            var userId = Guid.NewGuid();
            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User?)null);


            var result = await _adminService.GetUserByIdAsync(userId);


            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateUserPasswordAsync_WithValidData_ShouldUpdatePassword()
        {

            var userId = Guid.NewGuid();
            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = userId };
            var successResult = IdentityResult.Success;

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("reset-token");
            _userManagerMock.Setup(x => x.ResetPasswordAsync(user, "reset-token", "newpass"))
                .ReturnsAsync(successResult);


            var result = await _adminService.UpdateUserPasswordAsync(userId, "newpass");


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UpdateUserPasswordAsync_WithInvalidUser_ShouldReturnFalse()
        {

            var userId = Guid.NewGuid();
            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User?)null);


            var result = await _adminService.UpdateUserPasswordAsync(userId, "newpass");


            Assert.That(result, Is.False);
        }

        [Test]
        public async Task UpdateUserDetailsAsync_WithValidData_ShouldUpdateUser()
        {

            var userId = Guid.NewGuid();
            var user = new User("old@test.com", "olduser", "Old", "Name") { Id = userId };
            var successResult = IdentityResult.Success;

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(successResult);


            var result = await _adminService.UpdateUserDetailsAsync(userId, "New", "Name", "new@test.com", "newuser");


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task UpdateUserDetailsAsync_WithInvalidUser_ShouldReturnFalse()
        {

            var userId = Guid.NewGuid();
            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User?)null);


            var result = await _adminService.UpdateUserDetailsAsync(userId, "New", "Name", "new@test.com", "newuser");


            Assert.That(result, Is.False);
        }

        [Test]
        public async Task DeleteUserAsync_WithValidUser_ShouldMarkAsDeleted()
        {

            var userId = Guid.NewGuid();
            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = userId, IsDeleted = false };
            var successResult = IdentityResult.Success;

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(successResult);


            var result = await _adminService.DeleteUserAsync(userId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task RecoverUserAsync_WithValidUser_ShouldMarkAsNotDeleted()
        {

            var userId = Guid.NewGuid();
            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = userId, IsDeleted = true };
            var successResult = IdentityResult.Success;

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(successResult);


            var result = await _adminService.RecoverUserAsync(userId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task WarnUserAsync_WithValidData_ShouldCreateWarning()
        {

            var userId = Guid.NewGuid();
            var adminUserId = Guid.NewGuid();
            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = userId };
            var admin = new User("admin@test.com", "admin", "Admin", "User") { Id = adminUserId, IsAdmin = true };
            var warning = new Warning("Warning message", userId, adminUserId);

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.FindByIdAsync(adminUserId.ToString()))
                .ReturnsAsync(admin);
            _warningRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Warning>()))
                .ReturnsAsync(warning);


            var result = await _adminService.WarnUserAsync(userId, "Warning message", adminUserId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task GetAllTeamsAsync_ShouldReturnAllTeams()
        {

            var teams = new List<Team>
            {
                new Team { Id = Guid.NewGuid(), Name = "Team 1", Description = "Description 1", IsOpen = true },
                new Team { Id = Guid.NewGuid(), Name = "Team 2", Description = "Description 2", IsOpen = false }
            };

            _teamRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(teams);


            var result = await _adminService.GetAllTeamsAsync();


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Team 1"));
        }

        [Test]
        public async Task GetTeamByIdAsync_WithValidId_ShouldReturnTeam()
        {

            var teamId = Guid.NewGuid();
            var team = new Team 
            { 
                Id = teamId, 
                Name = "Test Team", 
                Description = "Test Description", 
                IsOpen = true,
                Members = new List<TeamMember>(),
                Events = new List<Event>()
            };

            _teamRepositoryMock.Setup(x => x.GetByIdWithMembersAndEventsAsync(teamId))
                .ReturnsAsync(team);


            var result = await _adminService.GetTeamByIdAsync(teamId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(teamId));
            Assert.That(result.Name, Is.EqualTo("Test Team"));
        }

        [Test]
        public async Task DeleteTeamAsync_WithValidId_ShouldDeleteTeam()
        {

            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team" };

            _teamRepositoryMock.Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync(team);
            _teamRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Team>()))
                .Returns(Task.CompletedTask);


            var result = await _adminService.DeleteTeamAsync(teamId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task JoinTeamAsync_WithValidData_ShouldJoinTeam()
        {

            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team" };
            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = userId };
            var teamMember = new TeamMember { TeamId = teamId, UserId = userId, Role = TeamRole.Member };

            _teamRepositoryMock.Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync(team);
            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _teamMemberRepositoryMock.Setup(x => x.GetByTeamAndUserAsync(teamId, userId))
                .ReturnsAsync((TeamMember?)null);
            _teamMemberRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TeamMember>()))
                .ReturnsAsync(teamMember);


            var result = await _adminService.JoinTeamAsync(teamId, userId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task KickUserFromTeamAsync_WithValidData_ShouldKickUser()
        {

            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var teamMember = new TeamMember { TeamId = teamId, UserId = userId, Role = TeamRole.Member };

            _teamMemberRepositoryMock.Setup(x => x.GetByTeamAndUserAsync(teamId, userId))
                .ReturnsAsync(teamMember);
            _teamMemberRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<TeamMember>()))
                .Returns(Task.CompletedTask);


            var result = await _adminService.KickUserFromTeamAsync(teamId, userId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ClearTeamChatAsync_WithValidTeamId_ShouldClearChat()
        {

            var teamId = Guid.NewGuid();
            var chats = new List<Chat>
            {
                new Chat(teamId, Guid.NewGuid(), "Message 1"),
                new Chat(teamId, Guid.NewGuid(), "Message 2")
            };

            _chatRepositoryMock.Setup(x => x.FindAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Chat, bool>>>()))
                .ReturnsAsync(chats);
            _chatRepositoryMock.Setup(x => x.DeleteRangeAsync(It.IsAny<IEnumerable<Chat>>()))
                .Returns(Task.CompletedTask);


            var result = await _adminService.ClearTeamChatAsync(teamId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task GetAllEventsAsync_ShouldReturnAllEvents()
        {

            var events = new List<Event>
            {
                new Event { Id = Guid.NewGuid(), Name = "Event 1", Description = "Description 1" },
                new Event { Id = Guid.NewGuid(), Name = "Event 2", Description = "Description 2" }
            };

            _eventRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(events);


            var result = await _adminService.GetAllEventsAsync();


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Event 1"));
        }

        [Test]
        public async Task GetEventByIdAsync_WithValidId_ShouldReturnEvent()
        {

            var eventId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            var eventItem = new Event 
            { 
                Id = eventId, 
                Name = "Test Event", 
                Description = "Test Description",
                Location = "Test Location",
                Date = DateTime.UtcNow,
                TeamId = teamId,
                CreatedBy = Guid.NewGuid()
            };

            _eventRepositoryMock.Setup(x => x.GetByIdWithTeamAsync(eventId))
                .ReturnsAsync(eventItem);


            var result = await _adminService.GetEventByIdAsync(eventId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(eventId));
            Assert.That(result.Name, Is.EqualTo("Test Event"));
        }

        [Test]
        public async Task DeleteEventAsync_WithValidId_ShouldDeleteEvent()
        {

            var eventId = Guid.NewGuid();
            var eventItem = new Event { Id = eventId, Name = "Test Event" };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId))
                .ReturnsAsync(eventItem);
            _eventRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);


            var result = await _adminService.DeleteEventAsync(eventId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task GetAllSupportMessagesAsync_ShouldReturnSupportMessages()
        {

            var userId = Guid.NewGuid();
            var user = new User("user@test.com", "user", "User", "Name") { Id = userId };
            var messages = new List<SupportMessage>
            {
                new SupportMessage("Subject 1", "Message 1", userId) { User = user },
                new SupportMessage("Subject 2", "Message 2", userId) { User = user }
            };

            _supportMessageRepositoryMock.Setup(x => x.GetActiveMessagesAsync())
                .ReturnsAsync(messages);


            var result = await _adminService.GetAllSupportMessagesAsync();


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Subject, Is.EqualTo("Subject 1"));
        }

        [Test]
        public async Task GetSupportMessageByIdAsync_WithValidId_ShouldReturnMessage()
        {

            var messageId = Guid.NewGuid();
            var message = new SupportMessage("Test Subject", "Test Message", Guid.NewGuid()) 
            { 
                Id = messageId
            };

            _supportMessageRepositoryMock.Setup(x => x.GetByIdAsync(messageId))
                .ReturnsAsync(message);


            var result = await _adminService.GetSupportMessageByIdAsync(messageId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(messageId));
            Assert.That(result.Subject, Is.EqualTo("Test Subject"));
        }

        [Test]
        public async Task MarkSupportMessageAsReadAsync_WithValidId_ShouldMarkAsRead()
        {

            var messageId = Guid.NewGuid();
            var message = new SupportMessage("Test", "Test", Guid.NewGuid()) { Id = messageId };

            _supportMessageRepositoryMock.Setup(x => x.GetByIdAsync(messageId))
                .ReturnsAsync(message);
            _supportMessageRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<SupportMessage>()))
                .Returns(Task.CompletedTask);


            var result = await _adminService.MarkSupportMessageAsReadAsync(messageId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ToggleSupportMessageFavoriteAsync_WithValidId_ShouldToggleFavorite()
        {

            var messageId = Guid.NewGuid();
            var message = new SupportMessage("Test", "Test", Guid.NewGuid()) { Id = messageId };

            _supportMessageRepositoryMock.Setup(x => x.GetByIdAsync(messageId))
                .ReturnsAsync(message);
            _supportMessageRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<SupportMessage>()))
                .Returns(Task.CompletedTask);


            var result = await _adminService.ToggleSupportMessageFavoriteAsync(messageId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task MarkSupportMessageAsCompletedAsync_WithValidId_ShouldToggleCompleted()
        {

            var messageId = Guid.NewGuid();
            var message = new SupportMessage("Test", "Test", Guid.NewGuid()) { Id = messageId };

            _supportMessageRepositoryMock.Setup(x => x.GetByIdAsync(messageId))
                .ReturnsAsync(message);
            _supportMessageRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<SupportMessage>()))
                .Returns(Task.CompletedTask);


            var result = await _adminService.MarkSupportMessageAsCompletedAsync(messageId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteSupportMessageAsync_WithValidId_ShouldMarkAsDeleted()
        {

            var messageId = Guid.NewGuid();
            var message = new SupportMessage("Test", "Test", Guid.NewGuid()) { Id = messageId };

            _supportMessageRepositoryMock.Setup(x => x.GetByIdAsync(messageId))
                .ReturnsAsync(message);
            _supportMessageRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<SupportMessage>()))
                .Returns(Task.CompletedTask);


            var result = await _adminService.DeleteSupportMessageAsync(messageId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CreateAnnouncementAsync_WithValidData_ShouldCreateAnnouncement()
        {

            var adminUserId = Guid.NewGuid();
            var admin = new User("admin@test.com", "admin", "Admin", "User") { Id = adminUserId, IsAdmin = true };
            var request = new CreateAnnouncementRequest
            {
                Title = "Test Announcement",
                Message = "Test Message"
            };
            var announcement = new Announcement("Test Announcement", "Test Message", adminUserId);

            _userManagerMock.Setup(x => x.FindByIdAsync(adminUserId.ToString()))
                .ReturnsAsync(admin);
            _announcementRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Announcement>()))
                .ReturnsAsync(announcement);


            var result = await _adminService.CreateAnnouncementAsync(request, adminUserId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Title, Is.EqualTo("Test Announcement"));
            Assert.That(result.Message, Is.EqualTo("Test Message"));
        }

        [Test]
        public async Task CreateAnnouncementAsync_WithNonAdminUser_ShouldThrowException()
        {

            var adminUserId = Guid.NewGuid();
            var user = new User("user@test.com", "user", "User", "Name") { Id = adminUserId, IsAdmin = false };
            var request = new CreateAnnouncementRequest
            {
                Title = "Test Announcement",
                Message = "Test Message"
            };

            _userManagerMock.Setup(x => x.FindByIdAsync(adminUserId.ToString()))
                .ReturnsAsync(user);

 & Assert
            var ex = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _adminService.CreateAnnouncementAsync(request, adminUserId));

            Assert.That(ex.Message, Does.Contain("Only admins can create announcements"));
        }

        [Test]
        public async Task GetAllAnnouncementsAsync_ShouldReturnAnnouncements()
        {

            var adminUserId = Guid.NewGuid();
            var adminUser = new User("admin@test.com", "admin", "Admin", "User") { Id = adminUserId };
            var announcements = new List<Announcement>
            {
                new Announcement("Title 1", "Message 1", adminUserId) { Id = Guid.NewGuid(), CreatedByUser = adminUser },
                new Announcement("Title 2", "Message 2", adminUserId) { Id = Guid.NewGuid(), CreatedByUser = adminUser }
            };

            _announcementRepositoryMock.Setup(x => x.GetAllAnnouncementsAsync())
                .ReturnsAsync(announcements);


            var result = await _adminService.GetAllAnnouncementsAsync();


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Title, Is.EqualTo("Title 1"));
        }

        [Test]
        public async Task DeleteAnnouncementAsync_WithValidId_ShouldMarkAsDeleted()
        {

            var announcementId = Guid.NewGuid();
            var announcement = new Announcement("Test Title", "Test Message", Guid.NewGuid()) 
            { 
                Id = announcementId,
                IsDeleted = false
            };

            _announcementRepositoryMock.Setup(x => x.GetByIdAsync(announcementId))
                .ReturnsAsync(announcement);
            _announcementRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Announcement>()))
                .Returns(Task.CompletedTask);


            var result = await _adminService.DeleteAnnouncementAsync(announcementId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ToggleAnnouncementActiveAsync_WithValidId_ShouldToggleActive()
        {

            var announcementId = Guid.NewGuid();
            var announcement = new Announcement("Test Title", "Test Message", Guid.NewGuid()) 
            { 
                Id = announcementId,
                IsActive = false
            };

            _announcementRepositoryMock.Setup(x => x.GetByIdAsync(announcementId))
                .ReturnsAsync(announcement);
            _announcementRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Announcement>()))
                .Returns(Task.CompletedTask);


            var result = await _adminService.ToggleAnnouncementActiveAsync(announcementId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ActivateAllAnnouncementsAsync_ShouldActivateAllAnnouncements()
        {

            var announcements = new List<Announcement>
            {
                new Announcement("Title 1", "Message 1", Guid.NewGuid()) { Id = Guid.NewGuid(), IsActive = false },
                new Announcement("Title 2", "Message 2", Guid.NewGuid()) { Id = Guid.NewGuid(), IsActive = false }
            };

            _announcementRepositoryMock.Setup(x => x.GetAllAnnouncementsAsync())
                .ReturnsAsync(announcements);
            _announcementRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Announcement>()))
                .Returns(Task.CompletedTask);


            var result = await _adminService.ActivateAllAnnouncementsAsync();


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task CreateWarningAsync_WithValidData_ShouldCreateWarning()
        {

            var userId = Guid.NewGuid();
            var adminUserId = Guid.NewGuid();
            var user = new User("user@test.com", "user", "User", "Name") { Id = userId };
            var admin = new User("admin@test.com", "admin", "Admin", "User") { Id = adminUserId, IsAdmin = true };
            var request = new CreateWarningRequest
            {
                Message = "Warning message"
            };
            var warning = new Warning("Warning message", userId, adminUserId);

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.FindByIdAsync(adminUserId.ToString()))
                .ReturnsAsync(admin);
            _warningRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Warning>()))
                .ReturnsAsync(warning);


            var result = await _adminService.CreateWarningAsync(request, userId, adminUserId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Message, Is.EqualTo("Warning message"));
            Assert.That(result.UserId, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetAllWarningsAsync_ShouldReturnWarnings()
        {

            var warnings = new List<Warning>
            {
                new Warning("Warning 1", Guid.NewGuid(), Guid.NewGuid()) { Id = Guid.NewGuid() },
                new Warning("Warning 2", Guid.NewGuid(), Guid.NewGuid()) { Id = Guid.NewGuid() }
            };

            _warningRepositoryMock.Setup(x => x.GetActiveWarningsAsync())
                .ReturnsAsync(warnings);


            var result = await _adminService.GetAllWarningsAsync();


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Message, Is.EqualTo("Warning 1"));
        }

        [Test]
        public async Task DeleteWarningAsync_WithValidId_ShouldMarkAsDeleted()
        {

            var warningId = Guid.NewGuid();
            var warning = new Warning("Test Warning", Guid.NewGuid(), Guid.NewGuid()) 
            { 
                Id = warningId,
                IsDeleted = false
            };

            _warningRepositoryMock.Setup(x => x.GetByIdAsync(warningId))
                .ReturnsAsync(warning);
            _warningRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Warning>()))
                .Returns(Task.CompletedTask);


            var result = await _adminService.DeleteWarningAsync(warningId);


            Assert.That(result, Is.True);
        }
    }
}

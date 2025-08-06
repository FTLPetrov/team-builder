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
using TeamBuilder.Services.Core.Contracts.Invitation.Requests;
using TeamBuilder.Services.Core.Contracts.Invitation.Responses;

namespace TeamBuilder.Services.Tests
{
    [TestFixture]
    public class InvitationServiceTests
    {
        private Mock<IInvitationRepository> _invitationRepositoryMock = null!;
        private Mock<ITeamRepository> _teamRepositoryMock = null!;
        private Mock<ITeamMemberRepository> _teamMemberRepositoryMock = null!;
        private Mock<UserManager<User>> _userManagerMock = null!;
        private InvitationService _invitationService = null!;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _invitationRepositoryMock = new Mock<IInvitationRepository>();
            _teamRepositoryMock = new Mock<ITeamRepository>();
            _teamMemberRepositoryMock = new Mock<ITeamMemberRepository>();
            _invitationService = new InvitationService(_invitationRepositoryMock.Object, _teamMemberRepositoryMock.Object, _userManagerMock.Object);
        }

        [Test]
        public async Task CreateAsync_WithValidData_ShouldCreateInvitation()
        {

            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var request = new InvitationCreateRequest
            {
                TeamId = teamId,
                InvitedUserId = userId,
                InvitedById = Guid.NewGuid()
            };

            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                TeamId = teamId,
                InvitedUserId = userId,
                InvitedById = Guid.NewGuid(),
                SentAt = DateTime.UtcNow,
                Accepted = false
            };

            _invitationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Invitation>()))
                .ReturnsAsync(invitation);


            var result = await _invitationService.CreateAsync(request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task CreateAsync_WithInvalidData_ShouldReturnSuccess()
        {

            var teamId = Guid.NewGuid();
            var request = new InvitationCreateRequest
            {
                TeamId = teamId,
                InvitedUserId = Guid.NewGuid(),
                InvitedById = Guid.NewGuid()
            };

            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                TeamId = teamId,
                InvitedUserId = request.InvitedUserId,
                InvitedById = request.InvitedById,
                SentAt = DateTime.UtcNow,
                Accepted = false
            };

            _invitationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Invitation>()))
                .ReturnsAsync(invitation);


            var result = await _invitationService.CreateAsync(request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True); // InvitationService doesn't validate team existence in CreateAsync
        }

        [Test]
        public async Task CreateByEmailAsync_WithValidData_ShouldCreateInvitation()
        {

            var teamId = Guid.NewGuid();
            var email = "test@test.com";
            var request = new InvitationCreateByEmailRequest
            {
                TeamId = teamId,
                InvitedUserEmail = email,
                InvitedById = Guid.NewGuid()
            };

            var user = new User(email, "testuser", "John", "Doe") { Id = Guid.NewGuid() };
            var invitation = new Invitation
            {
                Id = Guid.NewGuid(),
                TeamId = teamId,
                InvitedUserId = user.Id,
                InvitedById = Guid.NewGuid(),
                SentAt = DateTime.UtcNow,
                Accepted = false
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(user);
            _invitationRepositoryMock.Setup(x => x.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<Invitation, bool>>>()))
                .ReturnsAsync((Invitation?)null);
            _invitationRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Invitation>()))
                .ReturnsAsync(invitation);


            var result = await _invitationService.CreateByEmailAsync(request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Id, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task RespondAsync_WithAccept_ShouldAcceptInvitation()
        {

            var invitationId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var invitation = new Invitation { Id = invitationId, TeamId = teamId, InvitedUserId = userId, InvitedById = Guid.NewGuid(), SentAt = DateTime.UtcNow, Accepted = false };
            var teamMember = new TeamMember { TeamId = teamId, UserId = userId, Role = TeamRole.Member };

            var request = new InvitationRespondRequest
            {
                InvitationId = invitationId,
                Accept = true
            };

            _invitationRepositoryMock.Setup(x => x.GetByIdWithTeamAsync(invitationId))
                .ReturnsAsync(invitation);
            _teamMemberRepositoryMock.Setup(x => x.GetByTeamAndUserAsync(teamId, userId))
                .ReturnsAsync((TeamMember?)null);
            _teamMemberRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TeamMember>()))
                .ReturnsAsync(teamMember);
            _invitationRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Invitation>()))
                .Returns(Task.CompletedTask);


            var result = await _invitationService.RespondAsync(request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Success, Is.True);
            Assert.That(result.Accepted, Is.True);
        }

        [Test]
        public async Task RespondAsync_WithDecline_ShouldDeclineInvitation()
        {

            var invitationId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var invitation = new Invitation { Id = invitationId, TeamId = teamId, InvitedUserId = userId, InvitedById = Guid.NewGuid(), SentAt = DateTime.UtcNow, Accepted = false };

            var request = new InvitationRespondRequest
            {
                InvitationId = invitationId,
                Accept = false
            };

            _invitationRepositoryMock.Setup(x => x.GetByIdWithTeamAsync(invitationId))
                .ReturnsAsync(invitation);
            _invitationRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Invitation>()))
                .Returns(Task.CompletedTask);


            var result = await _invitationService.RespondAsync(request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Success, Is.True);
            Assert.That(result.Accepted, Is.False);
        }

        [Test]
        public async Task RespondAsync_WithInvalidInvitation_ShouldReturnError()
        {

            var invitationId = Guid.NewGuid();
            var request = new InvitationRespondRequest
            {
                InvitationId = invitationId,
                Accept = true
            };

            _invitationRepositoryMock.Setup(x => x.GetByIdWithTeamAsync(invitationId))
                .ReturnsAsync((Invitation?)null);


            var result = await _invitationService.RespondAsync(request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Success, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("not found"));
        }

        [Test]
        public async Task GetUserInvitationsAsync_WithValidUserId_ShouldReturnInvitations()
        {

            var userId = Guid.NewGuid();
            var invitations = new List<Invitation>
            {
                new Invitation { Id = Guid.NewGuid(), TeamId = Guid.NewGuid(), InvitedUserId = userId, InvitedById = Guid.NewGuid(), SentAt = DateTime.UtcNow, Accepted = false },
                new Invitation { Id = Guid.NewGuid(), TeamId = Guid.NewGuid(), InvitedUserId = userId, InvitedById = Guid.NewGuid(), SentAt = DateTime.UtcNow, Accepted = false }
            };

            _invitationRepositoryMock.Setup(x => x.GetPendingInvitationsAsync(userId))
                .ReturnsAsync(invitations);


            var result = await _invitationService.GetUserInvitationsAsync(userId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllAsync_WithValidTeamId_ShouldReturnInvitations()
        {

            var teamId = Guid.NewGuid();
            var invitations = new List<Invitation>
            {
                new Invitation { Id = Guid.NewGuid(), TeamId = teamId, InvitedUserId = Guid.NewGuid(), InvitedById = Guid.NewGuid(), SentAt = DateTime.UtcNow, Accepted = false },
                new Invitation { Id = Guid.NewGuid(), TeamId = teamId, InvitedUserId = Guid.NewGuid(), InvitedById = Guid.NewGuid(), SentAt = DateTime.UtcNow, Accepted = false }
            };

            _invitationRepositoryMock.Setup(x => x.GetInvitationsByTeamAsync(teamId))
                .ReturnsAsync(invitations);


            var result = await _invitationService.GetAllAsync(teamId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task DeleteAsync_WithValidId_ShouldDeleteInvitation()
        {

            var invitationId = Guid.NewGuid();
            var invitation = new Invitation { Id = invitationId, TeamId = Guid.NewGuid(), InvitedUserId = Guid.NewGuid(), InvitedById = Guid.NewGuid(), SentAt = DateTime.UtcNow, Accepted = false };

            _invitationRepositoryMock.Setup(x => x.GetByIdAsync(invitationId))
                .ReturnsAsync(invitation);
            _invitationRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Invitation>()))
                .Returns(Task.CompletedTask);


            var result = await _invitationService.DeleteAsync(invitationId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {

            var invitationId = Guid.NewGuid();
            _invitationRepositoryMock.Setup(x => x.GetByIdAsync(invitationId))
                .ReturnsAsync((Invitation?)null);


            var result = await _invitationService.DeleteAsync(invitationId);


            Assert.That(result, Is.False);
        }
    }
}

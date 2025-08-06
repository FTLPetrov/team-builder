using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;
using TeamBuilder.Services.Core;
using TeamBuilder.Services.Core.Contracts.Team.Requests;
using TeamBuilder.Services.Core.Contracts.Team.Responses;

namespace TeamBuilder.Services.Tests
{
    [TestFixture]
    public class TeamServiceTests
    {
        private Mock<ITeamRepository> _teamRepositoryMock = null!;
        private Mock<ITeamMemberRepository> _teamMemberRepositoryMock = null!;
        private Mock<IEventRepository> _eventRepositoryMock = null!;
        private TeamService _teamService = null!;

        [SetUp]
        public void Setup()
        {
            _teamRepositoryMock = new Mock<ITeamRepository>();
            _teamMemberRepositoryMock = new Mock<ITeamMemberRepository>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _teamService = new TeamService(_teamRepositoryMock.Object, _teamMemberRepositoryMock.Object, _eventRepositoryMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllTeams()
        {

            var teams = new List<Team>
            {
                new Team { Id = Guid.NewGuid(), Name = "Team 1", Description = "Description 1", IsOpen = true },
                new Team { Id = Guid.NewGuid(), Name = "Team 2", Description = "Description 2", IsOpen = false }
            };

            _teamRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(teams);


            var result = await _teamService.GetAllAsync();


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Team 1"));
        }

        [Test]
        public async Task GetByIdAsync_WithValidId_ShouldReturnTeam()
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


            var result = await _teamService.GetByIdAsync(teamId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(teamId));
            Assert.That(result.Name, Is.EqualTo("Test Team"));
        }

        [Test]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {

            var teamId = Guid.NewGuid();
            _teamRepositoryMock.Setup(x => x.GetByIdWithMembersAndEventsAsync(teamId))
                .ReturnsAsync((Team?)null);


            var result = await _teamService.GetByIdAsync(teamId);


            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateAsync_WithValidData_ShouldCreateTeam()
        {

            var request = new TeamCreateRequest
            {
                Name = "Test Team",
                Description = "Test Description",
                IsOpen = true
            };

            var team = new Team 
            { 
                Id = Guid.NewGuid(),
                Name = "Test Team",
                Description = "Test Description",
                IsOpen = true
            };

            _teamRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Team>()))
                .ReturnsAsync(team);


            var result = await _teamService.CreateAsync(request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Test Team"));
            Assert.That(result.Description, Is.EqualTo("Test Description"));
        }

        [Test]
        public async Task UpdateAsync_WithValidData_ShouldUpdateTeam()
        {

            var teamId = Guid.NewGuid();
            var team = new Team 
            { 
                Id = teamId, 
                Name = "Old Team", 
                Description = "Old Description",
                OrganizerId = Guid.NewGuid()
            };
            var request = new TeamUpdateRequest
            {
                Name = "Updated Team",
                Description = "Updated Description",
                IsOpen = false
            };

            _teamRepositoryMock.Setup(x => x.GetByIdWithMembersAndEventsAsync(teamId))
                .ReturnsAsync(team);
            _teamRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Team>()))
                .Returns(Task.CompletedTask);


            var result = await _teamService.UpdateAsync(teamId, request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Updated Team"));
            Assert.That(result.Description, Is.EqualTo("Updated Description"));
        }

        [Test]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
        {

            var teamId = Guid.NewGuid();
            var request = new TeamUpdateRequest
            {
                Name = "Updated Team",
                Description = "Updated Description"
            };

            _teamRepositoryMock.Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync((Team?)null);


            var result = await _teamService.UpdateAsync(teamId, request);


            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WithValidId_ShouldDeleteTeam()
        {

            var teamId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team" };

            _teamRepositoryMock.Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync(team);
            _teamRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Team>()))
                .Returns(Task.CompletedTask);


            var result = await _teamService.DeleteAsync(teamId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {

            var teamId = Guid.NewGuid();
            _teamRepositoryMock.Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync((Team?)null);


            var result = await _teamService.DeleteAsync(teamId);


            Assert.That(result, Is.False);
        }

        [Test]
        public async Task JoinTeamAsync_WithValidData_ShouldJoinTeam()
        {

            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var team = new Team { 
                Id = teamId, 
                Name = "Test Team", 
                IsOpen = true,
                Members = new List<TeamMember>() // Empty members list
            };
            var teamMember = new TeamMember { TeamId = teamId, UserId = userId, Role = TeamRole.Member };

            _teamRepositoryMock.Setup(x => x.GetByIdWithMembersAsync(teamId))
                .ReturnsAsync(team);
            _teamMemberRepositoryMock.Setup(x => x.AddAsync(It.IsAny<TeamMember>()))
                .ReturnsAsync(teamMember);


            var result = await _teamService.JoinTeamAsync(teamId, userId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task JoinTeamAsync_WithClosedTeam_ShouldReturnFalse()
        {

            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsOpen = false };

            _teamRepositoryMock.Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync(team);


            var result = await _teamService.JoinTeamAsync(teamId, userId);


            Assert.That(result, Is.False);
        }

        [Test]
        public async Task JoinTeamAsync_WithExistingMember_ShouldReturnFalse()
        {

            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var team = new Team { Id = teamId, Name = "Test Team", IsOpen = true };
            var existingMember = new TeamMember { TeamId = teamId, UserId = userId, Role = TeamRole.Member };

            _teamRepositoryMock.Setup(x => x.GetByIdAsync(teamId))
                .ReturnsAsync(team);
            _teamMemberRepositoryMock.Setup(x => x.GetByTeamAndUserAsync(teamId, userId))
                .ReturnsAsync(existingMember);


            var result = await _teamService.JoinTeamAsync(teamId, userId);


            Assert.That(result, Is.False);
        }

        [Test]
        public async Task LeaveTeamAsync_WithValidData_ShouldLeaveTeam()
        {

            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var teamMember = new TeamMember { TeamId = teamId, UserId = userId, Role = TeamRole.Member };
            var team = new Team { 
                Id = teamId, 
                Name = "Test Team", 
                Members = new List<TeamMember> { teamMember } // User is a member
            };

            _teamRepositoryMock.Setup(x => x.GetByIdWithMembersAsync(teamId))
                .ReturnsAsync(team);
            _teamMemberRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<TeamMember>()))
                .Returns(Task.CompletedTask);


            var result = await _teamService.LeaveTeamAsync(teamId, userId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task LeaveTeamAsync_WithNonMember_ShouldReturnFalse()
        {

            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _teamMemberRepositoryMock.Setup(x => x.GetByTeamAndUserAsync(teamId, userId))
                .ReturnsAsync((TeamMember?)null);


            var result = await _teamService.LeaveTeamAsync(teamId, userId);


            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetByIdAsync_WithValidTeamId_ShouldReturnTeamWithMembers()
        {

            var teamId = Guid.NewGuid();
            var members = new List<TeamMember>
            {
                new TeamMember { TeamId = teamId, UserId = Guid.NewGuid(), Role = TeamRole.Organizer },
                new TeamMember { TeamId = teamId, UserId = Guid.NewGuid(), Role = TeamRole.Member }
            };
            var team = new Team 
            { 
                Id = teamId, 
                Name = "Test Team", 
                Description = "Test Description", 
                IsOpen = true,
                Members = members,
                Events = new List<Event>()
            };

            _teamRepositoryMock.Setup(x => x.GetByIdWithMembersAndEventsAsync(teamId))
                .ReturnsAsync(team);


            var result = await _teamService.GetByIdAsync(teamId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Members.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task GetByIdAsync_WithValidTeamId_ShouldReturnTeamWithEvents()
        {

            var teamId = Guid.NewGuid();
            var events = new List<Event>
            {
                new Event { Id = Guid.NewGuid(), Name = "Event 1", TeamId = teamId },
                new Event { Id = Guid.NewGuid(), Name = "Event 2", TeamId = teamId }
            };
            var team = new Team 
            { 
                Id = teamId, 
                Name = "Test Team", 
                Description = "Test Description", 
                IsOpen = true,
                Members = new List<TeamMember>(),
                Events = events
            };

            _teamRepositoryMock.Setup(x => x.GetByIdWithMembersAndEventsAsync(teamId))
                .ReturnsAsync(team);


            var result = await _teamService.GetByIdAsync(teamId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Events.Count(), Is.EqualTo(2));
            Assert.That(result.Events.First().Name, Is.EqualTo("Event 1"));
        }

        [Test]
        public async Task GetUserTeamsAsync_WithValidUserId_ShouldReturnTeams()
        {

            var userId = Guid.NewGuid();
            var teams = new List<Team>
            {
                new Team { Id = Guid.NewGuid(), Name = "User Team 1" },
                new Team { Id = Guid.NewGuid(), Name = "User Team 2" }
            };

            _teamRepositoryMock.Setup(x => x.GetTeamsByMemberAsync(userId))
                .ReturnsAsync(teams);


            var result = await _teamService.GetUserTeamsAsync(userId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("User Team 1"));
        }
    }
}

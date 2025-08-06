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
    public class EventServiceTests
    {
        private Mock<IEventRepository> _eventRepositoryMock = null!;
        private Mock<ITeamRepository> _teamRepositoryMock = null!;
        private Mock<IEventParticipationRepository> _eventParticipationRepositoryMock = null!;
        private EventService _eventService = null!;

        [SetUp]
        public void Setup()
        {
            _eventRepositoryMock = new Mock<IEventRepository>();
            _teamRepositoryMock = new Mock<ITeamRepository>();
            _eventParticipationRepositoryMock = new Mock<IEventParticipationRepository>();
            _eventService = new EventService(_eventRepositoryMock.Object, _teamRepositoryMock.Object, _eventParticipationRepositoryMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllEvents()
        {

            var events = new List<Event>
            {
                new Event { Id = Guid.NewGuid(), Name = "Event 1", Description = "Description 1" },
                new Event { Id = Guid.NewGuid(), Name = "Event 2", Description = "Description 2" }
            };

            _eventRepositoryMock.Setup(x => x.GetAllAsync())
                .ReturnsAsync(events);


            var result = await _eventService.GetAllAsync();


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Event 1"));
        }

        [Test]
        public async Task GetByIdAsync_WithValidId_ShouldReturnEvent()
        {

            var eventId = Guid.NewGuid();
            var eventItem = new Event 
            { 
                Id = eventId, 
                Name = "Test Event", 
                Description = "Test Description",
                Location = "Test Location",
                Date = DateTime.UtcNow
            };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId))
                .ReturnsAsync(eventItem);


            var result = await _eventService.GetByIdAsync(eventId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(eventId));
            Assert.That(result.Name, Is.EqualTo("Test Event"));
        }

        [Test]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {

            var eventId = Guid.NewGuid();
            _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId))
                .ReturnsAsync((Event?)null);


            var result = await _eventService.GetByIdAsync(eventId);


            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateAsync_WithValidData_ShouldCreateEvent()
        {

            var teamId = Guid.NewGuid();
            var request = new EventCreateRequest
            {
                Name = "Test Event",
                Description = "Test Description",
                Location = "Test Location",
                Date = DateTime.UtcNow,
                TeamId = teamId,
                CreatedBy = Guid.NewGuid()
            };

            var eventItem = new Event 
            { 
                Id = Guid.NewGuid(),
                Name = "Test Event",
                Description = "Test Description",
                Location = "Test Location",
                Date = DateTime.UtcNow,
                TeamId = teamId,
                CreatedBy = Guid.NewGuid()
            };

            _eventRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Event>()))
                .ReturnsAsync(eventItem);


            var result = await _eventService.CreateAsync(request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.True);
            Assert.That(result.Name, Is.EqualTo("Test Event"));
            Assert.That(result.Description, Is.EqualTo("Test Description"));
        }

        [Test]
        public async Task CreateAsync_WithInvalidRequest_ShouldReturnFailure()
        {

            var request = new EventCreateRequest
            {
                Name = "",
                Description = "Test Description",
                Date = default,
                TeamId = Guid.NewGuid(),
                CreatedBy = Guid.NewGuid()
            };


            var result = await _eventService.CreateAsync(request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorMessage, Does.Contain("required"));
        }

        [Test]
        public async Task UpdateAsync_WithValidData_ShouldUpdateEvent()
        {

            var eventId = Guid.NewGuid();
            var eventItem = new Event 
            { 
                Id = eventId, 
                Name = "Old Event", 
                Description = "Old Description" 
            };
            var request = new EventUpdateRequest
            {
                Name = "Updated Event",
                Description = "Updated Description",
                Location = "Updated Location"
            };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId))
                .ReturnsAsync(eventItem);
            _eventRepositoryMock.Setup(x => x.UpdateAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);


            var result = await _eventService.UpdateAsync(eventId, request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Name, Is.EqualTo("Updated Event"));
            Assert.That(result.Description, Is.EqualTo("Updated Description"));
        }

        [Test]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
        {

            var eventId = Guid.NewGuid();
            var request = new EventUpdateRequest
            {
                Name = "Updated Event",
                Description = "Updated Description"
            };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId))
                .ReturnsAsync((Event?)null);


            var result = await _eventService.UpdateAsync(eventId, request);


            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WithValidId_ShouldDeleteEvent()
        {

            var eventId = Guid.NewGuid();
            var eventItem = new Event { Id = eventId, Name = "Test Event" };

            _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId))
                .ReturnsAsync(eventItem);
            _eventRepositoryMock.Setup(x => x.DeleteAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);


            var result = await _eventService.DeleteAsync(eventId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {

            var eventId = Guid.NewGuid();
            _eventRepositoryMock.Setup(x => x.GetByIdAsync(eventId))
                .ReturnsAsync((Event?)null);


            var result = await _eventService.DeleteAsync(eventId);


            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetAllAsync_WithTeamId_ShouldReturnEvents()
        {

            var teamId = Guid.NewGuid();
            var events = new List<Event>
            {
                new Event { Id = Guid.NewGuid(), Name = "Event 1", TeamId = teamId, Date = DateTime.UtcNow, CreatedBy = Guid.NewGuid() },
                new Event { Id = Guid.NewGuid(), Name = "Event 2", TeamId = teamId, Date = DateTime.UtcNow, CreatedBy = Guid.NewGuid() }
            };

            _eventRepositoryMock.Setup(x => x.GetEventsByTeamAsync(teamId))
                .ReturnsAsync(events);
            _teamRepositoryMock.Setup(x => x.GetByIdWithMembersAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Team?)null);
            _eventParticipationRepositoryMock.Setup(x => x.GetParticipationsByEventAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<EventParticipation>());


            var result = await _eventService.GetAllAsync(teamId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Name, Is.EqualTo("Event 1"));
        }

        [Test]
        public async Task GetAllAsync_WithTeamId_WithEmptyResult_ShouldReturnEmptyList()
        {

            var teamId = Guid.NewGuid();
            var events = new List<Event>();

            _eventRepositoryMock.Setup(x => x.GetEventsByTeamAsync(teamId))
                .ReturnsAsync(events);


            var result = await _eventService.GetAllAsync(teamId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(0));
        }
    }
}

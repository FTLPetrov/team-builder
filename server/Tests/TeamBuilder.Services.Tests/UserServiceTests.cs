using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;
using TeamBuilder.Data.Models;
using TeamBuilder.Data.Repositories.Interfaces;
using TeamBuilder.Services.Core;
using TeamBuilder.Services.Core.Contracts.User.Requests;
using TeamBuilder.Services.Core.Contracts.User.Responses;
using TeamBuilder.Services.Core.Interfaces;

namespace TeamBuilder.Services.Tests
{
    [TestFixture]
    public class UserServiceTests
    {
        private Mock<UserManager<User>> _userManagerMock = null!;
        private Mock<IJwtService> _jwtServiceMock = null!;
        private Mock<IWarningRepository> _warningRepositoryMock = null!;
        private UserService _userService = null!;

        [SetUp]
        public void Setup()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);
            _jwtServiceMock = new Mock<IJwtService>();
            _warningRepositoryMock = new Mock<IWarningRepository>();
            _userService = new UserService(_userManagerMock.Object, _jwtServiceMock.Object, _warningRepositoryMock.Object);
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnAllUsers()
        {

            var users = new List<User>
            {
                new User("test1@test.com", "user1", "John", "Doe") { Id = Guid.NewGuid() },
                new User("test2@test.com", "user2", "Jane", "Smith") { Id = Guid.NewGuid() }
            };

            _userManagerMock.Setup(x => x.Users).Returns(users.AsQueryable());


            var result = await _userService.GetAllAsync();


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            Assert.That(result.First().Email, Is.EqualTo("test1@test.com"));
        }

        [Test]
        public async Task GetByIdAsync_WithValidId_ShouldReturnUser()
        {

            var userId = Guid.NewGuid();
            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = userId };

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);


            var result = await _userService.GetByIdAsync(userId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(userId));
            Assert.That(result.Email, Is.EqualTo("test@test.com"));
        }

        [Test]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {

            var userId = Guid.NewGuid();
            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User?)null);


            var result = await _userService.GetByIdAsync(userId);


            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task CreateAsync_WithValidData_ShouldCreateUser()
        {

            var user = new User("test@test.com", "testuser", "John", "Doe");
            var successResult = IdentityResult.Success;

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(successResult);


            var result = await _userService.CreateAsync("John", "Doe", "test@test.com", "testuser", "password123");


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Email, Is.EqualTo("test@test.com"));
            Assert.That(result.UserName, Is.EqualTo("testuser"));
        }

        [Test]
        public async Task CreateAsync_WithInvalidData_ShouldThrowException()
        {

            var errors = new List<IdentityError> { new IdentityError { Description = "Email already exists" } };
            var failureResult = IdentityResult.Failed(errors.ToArray());

            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(failureResult);

 & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _userService.CreateAsync("John", "Doe", "test@test.com", "testuser", "password123"));

            Assert.That(ex.Message, Does.Contain("Email already exists"));
        }

        [Test]
        public async Task UpdateAsync_WithValidData_ShouldUpdateUser()
        {

            var userId = Guid.NewGuid();
            var user = new User("old@test.com", "olduser", "Old", "Name") { Id = userId };
            var successResult = IdentityResult.Success;

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ReturnsAsync(successResult);


            var result = await _userService.UpdateAsync(userId, "New", "Name", "new@test.com", "newuser");


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.FirstName, Is.EqualTo("New"));
            Assert.That(result.LastName, Is.EqualTo("Name"));
        }

        [Test]
        public async Task UpdateAsync_WithInvalidId_ShouldReturnNull()
        {

            var userId = Guid.NewGuid();
            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User?)null);


            var result = await _userService.UpdateAsync(userId, "New", "Name", "new@test.com", "newuser");


            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task DeleteAsync_WithValidId_ShouldDeleteUser()
        {

            var userId = Guid.NewGuid();
            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = userId };
            var successResult = IdentityResult.Success;

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.DeleteAsync(It.IsAny<User>()))
                .ReturnsAsync(successResult);


            var result = await _userService.DeleteAsync(userId);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task DeleteAsync_WithInvalidId_ShouldReturnFalse()
        {

            var userId = Guid.NewGuid();
            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User?)null);


            var result = await _userService.DeleteAsync(userId);


            Assert.That(result, Is.False);
        }

        [Test]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnLoginResponse()
        {

            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = Guid.NewGuid() };
            var request = new UserLoginRequest { Email = "test@test.com", Password = "password123" };
            var token = "jwt-token";

            _userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "password123"))
                .ReturnsAsync(true);
            _jwtServiceMock.Setup(x => x.GenerateToken(user))
                .Returns(token);


            var result = await _userService.LoginAsync(request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Token, Is.EqualTo(token));
            Assert.That(result.User.Email, Is.EqualTo("test@test.com"));
        }

        [Test]
        public async Task LoginAsync_WithInvalidEmail_ShouldReturnNull()
        {

            var request = new UserLoginRequest { Email = "invalid@test.com", Password = "password123" };

            _userManagerMock.Setup(x => x.FindByEmailAsync("invalid@test.com"))
                .ReturnsAsync((User?)null);


            var result = await _userService.LoginAsync(request);


            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task LoginAsync_WithInvalidPassword_ShouldReturnNull()
        {

            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = Guid.NewGuid() };
            var request = new UserLoginRequest { Email = "test@test.com", Password = "wrongpassword" };

            _userManagerMock.Setup(x => x.FindByEmailAsync("test@test.com"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "wrongpassword"))
                .ReturnsAsync(false);


            var result = await _userService.LoginAsync(request);


            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task LoginAsync_WithUsername_ShouldFindUserByUsername()
        {

            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = Guid.NewGuid() };
            var request = new UserLoginRequest { Username = "testuser", Password = "password123" };
            var token = "jwt-token";

            _userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((User?)null);
            _userManagerMock.Setup(x => x.FindByNameAsync("testuser"))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "password123"))
                .ReturnsAsync(true);
            _jwtServiceMock.Setup(x => x.GenerateToken(user))
                .Returns(token);


            var result = await _userService.LoginAsync(request);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Token, Is.EqualTo(token));
        }

        [Test]
        public async Task LogoutAsync_WithValidToken_ShouldReturnTrue()
        {

            var token = "valid-token";


            var result = await _userService.LogoutAsync(token);


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task LogoutAsync_WithEmptyToken_ShouldReturnFalse()
        {

            var result = await _userService.LogoutAsync("");


            Assert.That(result, Is.False);
        }

        [Test]
        public void IsTokenBlacklisted_WithBlacklistedToken_ShouldReturnTrue()
        {

            var token = "blacklisted-token";
            _userService.LogoutAsync(token).Wait();


            var result = _userService.IsTokenBlacklisted(token);


            Assert.That(result, Is.True);
        }

        [Test]
        public void IsTokenBlacklisted_WithValidToken_ShouldReturnFalse()
        {

            var token = "valid-token";


            var result = _userService.IsTokenBlacklisted(token);


            Assert.That(result, Is.False);
        }

        [Test]
        public async Task ChangePasswordAsync_WithValidCredentials_ShouldReturnTrue()
        {

            var userId = Guid.NewGuid();
            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = userId };
            var successResult = IdentityResult.Success;

            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);
            _userManagerMock.Setup(x => x.ChangePasswordAsync(user, "oldpass", "newpass"))
                .ReturnsAsync(successResult);


            var result = await _userService.ChangePasswordAsync(userId, "oldpass", "newpass");


            Assert.That(result, Is.True);
        }

        [Test]
        public async Task ChangePasswordAsync_WithInvalidUser_ShouldReturnFalse()
        {

            var userId = Guid.NewGuid();
            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync((User?)null);


            var result = await _userService.ChangePasswordAsync(userId, "oldpass", "newpass");


            Assert.That(result, Is.False);
        }

        [Test]
        public async Task GetCurrentUserAsync_WithValidToken_ShouldReturnUser()
        {

            var userId = Guid.NewGuid();
            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = userId };
            var token = "valid-token";

            _jwtServiceMock.Setup(x => x.GetUserIdFromToken(token))
                .Returns(userId.ToString());
            _userManagerMock.Setup(x => x.FindByIdAsync(userId.ToString()))
                .ReturnsAsync(user);


            var result = await _userService.GetCurrentUserAsync(token);


            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Id, Is.EqualTo(userId));
        }

        [Test]
        public async Task GetCurrentUserAsync_WithBlacklistedToken_ShouldReturnNull()
        {

            var token = "blacklisted-token";
            await _userService.LogoutAsync(token);


            var result = await _userService.GetCurrentUserAsync(token);


            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetCurrentUserAsync_WithInvalidToken_ShouldReturnNull()
        {

            var token = "invalid-token";
            _jwtServiceMock.Setup(x => x.GetUserIdFromToken(token))
                .Returns((string?)null);


            var result = await _userService.GetCurrentUserAsync(token);


            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetUserWarningsAsync_ShouldReturnWarnings()
        {

            var userId = Guid.NewGuid();
            var adminId = Guid.NewGuid();
            var warnings = new List<Warning>
            {
                new Warning("Warning 1", userId, adminId),
                new Warning("Warning 2", userId, adminId)
            };

            _warningRepositoryMock.Setup(x => x.GetWarningsByUserAsync(userId))
                .ReturnsAsync(warnings);


            var result = await _userService.GetUserWarningsAsync(userId);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }
    }
}

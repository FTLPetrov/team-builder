using System;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using TeamBuilder.Data.Models;
using TeamBuilder.Services.Core;

namespace TeamBuilder.Services.Tests
{
    [TestFixture]
    public class JwtServiceTests
    {
        private Mock<IConfiguration> _configurationMock = null!;
        private JwtService _jwtService = null!;

        [SetUp]
        public void Setup()
        {
            _configurationMock = new Mock<IConfiguration>();
            var jwtSettingsSection = new Mock<IConfigurationSection>();
            jwtSettingsSection.Setup(x => x["SecretKey"]).Returns("your-super-secret-key-with-at-least-32-characters");
            jwtSettingsSection.Setup(x => x["Issuer"]).Returns("TeamBuilder");
            jwtSettingsSection.Setup(x => x["Audience"]).Returns("TeamBuilderUsers");
            jwtSettingsSection.Setup(x => x["ExpirationInMinutes"]).Returns("60");
            _configurationMock.Setup(x => x.GetSection("JwtSettings")).Returns(jwtSettingsSection.Object);

            _jwtService = new JwtService(_configurationMock.Object);
        }

        [Test]
        public void GenerateToken_WithValidUser_ShouldReturnToken()
        {

            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = Guid.NewGuid() };


            var result = _jwtService.GenerateToken(user);


            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.Empty);
        }

        [Test]
        public void GenerateToken_WithNullUser_ShouldThrowException()
        {
 & Assert
            var ex = Assert.Throws<NullReferenceException>(() => _jwtService.GenerateToken(null!));
            Assert.That(ex, Is.Not.Null);
        }

        [Test]
        public void GetUserIdFromToken_WithValidToken_ShouldReturnUserId()
        {

            var userId = Guid.NewGuid();
            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = userId };
            var token = _jwtService.GenerateToken(user);


            var result = _jwtService.GetUserIdFromToken(token);


            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(userId.ToString()));
        }

        [Test]
        public void GetUserIdFromToken_WithInvalidToken_ShouldReturnNull()
        {

            var invalidToken = "invalid-token";


            var result = _jwtService.GetUserIdFromToken(invalidToken);


            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUserIdFromToken_WithEmptyToken_ShouldReturnNull()
        {

            var result = _jwtService.GetUserIdFromToken("");


            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetUserIdFromToken_WithNullToken_ShouldReturnNull()
        {

            var result = _jwtService.GetUserIdFromToken(null!);


            Assert.That(result, Is.Null);
        }

        [Test]
        public void ValidateToken_WithValidToken_ShouldReturnPrincipal()
        {

            var user = new User("test@test.com", "testuser", "John", "Doe") { Id = Guid.NewGuid() };
            var token = _jwtService.GenerateToken(user);


            var result = _jwtService.ValidateToken(token);


            Assert.That(result, Is.Not.Null);
            Assert.That(result.Identity!.IsAuthenticated, Is.True);
        }

        [Test]
        public void ValidateToken_WithInvalidToken_ShouldReturnNull()
        {

            var invalidToken = "invalid-token";


            var result = _jwtService.ValidateToken(invalidToken);


            Assert.That(result, Is.Null);
        }

        [Test]
        public void ValidateToken_WithEmptyToken_ShouldReturnNull()
        {

            var result = _jwtService.ValidateToken("");


            Assert.That(result, Is.Null);
        }

        [Test]
        public void ValidateToken_WithNullToken_ShouldReturnNull()
        {

            var result = _jwtService.ValidateToken(null!);


            Assert.That(result, Is.Null);
        }
    }
}

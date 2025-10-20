using ManiaDeLimpeza.Api.Auth;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests
{
    [TestClass]
    public class UserFetchMiddlewareTests
    {
        private Mock<RequestDelegate> _nextMock = null!;
        private Mock<IUserService> _userServiceMock = null!;
        private Mock<ILogger<UserFetchMiddleware>> _loggerMock = null!;
        private UserFetchMiddleware _middleware = null!;
        private DefaultHttpContext _httpContext = null!;

        [TestInitialize]
        public void Setup()
        {
            _nextMock = new Mock<RequestDelegate>();
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<UserFetchMiddleware>>();
            _middleware = new UserFetchMiddleware(_nextMock.Object, _loggerMock.Object);
            _httpContext = new DefaultHttpContext();
        }

        [TestMethod]
        public async Task InvokeAsync_WhenUserNotAuthenticated_ShouldNotFetchUser()
        {
            // Arrange
            _httpContext.User = new ClaimsPrincipal(new ClaimsIdentity()); // Not authenticated

            // Act
            await _middleware.InvokeAsync(_httpContext, _userServiceMock.Object);

            // Assert
            _userServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            Assert.IsNull(_httpContext.Items["User"]);
            _nextMock.Verify(x => x(_httpContext), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_WhenUserAuthenticated_ShouldFetchUserFromDatabase()
        {
            // Arrange
            var userId = 123;
            var user = new User 
            { 
                Id = userId, 
                Name = "Test User", 
                Email = "test@example.com",
                CompanyId = 1,
                Profile = UserProfile.Admin,
                CreatedDate = DateTime.UtcNow
            };

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            _httpContext.User = new ClaimsPrincipal(identity);

            _userServiceMock.Setup(x => x.GetByIdAsync(userId))
                           .ReturnsAsync(user);

            // Act
            await _middleware.InvokeAsync(_httpContext, _userServiceMock.Object);

            // Assert
            _userServiceMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
            Assert.AreEqual(user, _httpContext.Items["User"]);
            _nextMock.Verify(x => x(_httpContext), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_WhenUserIdInvalid_ShouldNotFetchUser()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "invalid-id"),
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            _httpContext.User = new ClaimsPrincipal(identity);

            // Act
            await _middleware.InvokeAsync(_httpContext, _userServiceMock.Object);

            // Assert
            _userServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            Assert.IsNull(_httpContext.Items["User"]);
            _nextMock.Verify(x => x(_httpContext), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_WhenUserNotFoundInDatabase_ShouldNotSetUserInContext()
        {
            // Arrange
            var userId = 123;
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            _httpContext.User = new ClaimsPrincipal(identity);

            _userServiceMock.Setup(x => x.GetByIdAsync(userId))
                           .ReturnsAsync((User?)null);

            // Act
            await _middleware.InvokeAsync(_httpContext, _userServiceMock.Object);

            // Assert
            _userServiceMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
            Assert.IsNull(_httpContext.Items["User"]);
            _nextMock.Verify(x => x(_httpContext), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_WhenUserServiceThrowsException_ShouldContinueProcessing()
        {
            // Arrange
            var userId = 123;
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            _httpContext.User = new ClaimsPrincipal(identity);

            _userServiceMock.Setup(x => x.GetByIdAsync(userId))
                           .ThrowsAsync(new Exception("Database error"));

            // Act
            await _middleware.InvokeAsync(_httpContext, _userServiceMock.Object);

            // Assert
            _userServiceMock.Verify(x => x.GetByIdAsync(userId), Times.Once);
            Assert.IsNull(_httpContext.Items["User"]);
            _nextMock.Verify(x => x(_httpContext), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_WhenNoUserIdClaim_ShouldNotFetchUser()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, "test@example.com")
                // No NameIdentifier claim
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            _httpContext.User = new ClaimsPrincipal(identity);

            // Act
            await _middleware.InvokeAsync(_httpContext, _userServiceMock.Object);

            // Assert
            _userServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            Assert.IsNull(_httpContext.Items["User"]);
            _nextMock.Verify(x => x(_httpContext), Times.Once);
        }

        [TestMethod]
        public async Task InvokeAsync_WhenEmptyUserIdClaim_ShouldNotFetchUser()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, ""),
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            _httpContext.User = new ClaimsPrincipal(identity);

            // Act
            await _middleware.InvokeAsync(_httpContext, _userServiceMock.Object);

            // Assert
            _userServiceMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
            Assert.IsNull(_httpContext.Items["User"]);
            _nextMock.Verify(x => x(_httpContext), Times.Once);
        }
    }
}
using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests
{
    [TestClass]
    public class AuthBaseControllerTests
    {
        private TestAuthController _controller = null!;
        private User _testUser = null!;

        [TestInitialize]
        public void Setup()
        {
            _controller = new TestAuthController();
            _testUser = new User
            {
                Id = 1,
                Name = "Test User",
                Email = "test@example.com",
                CompanyId = 1,
                Profile = UserProfile.Admin,
                CreatedDate = DateTime.UtcNow
            };
        }

        [TestMethod]
        public void CurrentUser_WhenUserInContext_ShouldReturnUser()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Items["User"] = _testUser;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.GetCurrentUser();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testUser.Id, result.Id);
            Assert.AreEqual(_testUser.Email, result.Email);
        }

        [TestMethod]
        public void CurrentUser_WhenNoUserInContext_ShouldReturnNull()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.GetCurrentUser();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetCurrentUserId_WhenValidClaim_ShouldReturnUserId()
        {
            // Arrange
            var userId = 123;
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = principal
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.GetCurrentUserIdPublic();

            // Assert
            Assert.AreEqual(userId, result);
        }

        [TestMethod]
        public void GetCurrentUserId_WhenInvalidClaim_ShouldReturnZero()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "invalid-id"),
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = principal
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.GetCurrentUserIdPublic();

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetCurrentUserId_WhenNoClaim_ShouldReturnZero()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, "test@example.com")
                // No NameIdentifier claim
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = principal
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.GetCurrentUserIdPublic();

            // Assert
            Assert.AreEqual(0, result);
        }

        [TestMethod]
        public void GetCurrentUserEmail_WhenValidClaim_ShouldReturnEmail()
        {
            // Arrange
            var email = "test@example.com";
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123"),
                new Claim(ClaimTypes.Email, email)
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = principal
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.GetCurrentUserEmailPublic();

            // Assert
            Assert.AreEqual(email, result);
        }

        [TestMethod]
        public void GetCurrentUserEmail_WhenNoClaim_ShouldReturnNull()
        {
            // Arrange
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "123")
                // No Email claim
            };
            var identity = new ClaimsIdentity(claims, "Bearer");
            var principal = new ClaimsPrincipal(identity);

            var httpContext = new DefaultHttpContext
            {
                User = principal
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.GetCurrentUserEmailPublic();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void ValidateCurrentUser_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Items["User"] = _testUser;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.ValidateCurrentUserPublic();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(_testUser.Id, result.Value.Id);
        }

        [TestMethod]
        public void ValidateCurrentUser_WhenUserNotExists_ShouldReturnUnauthorized()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.ValidateCurrentUserPublic();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Result, typeof(UnauthorizedObjectResult));
            var unauthorizedResult = result.Result as UnauthorizedObjectResult;
            Assert.AreEqual("Unable to resolve current user", unauthorizedResult!.Value);
        }

        [TestMethod]
        public void GetCurrentUserOrThrow_WhenUserExists_ShouldReturnUser()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            httpContext.Items["User"] = _testUser;
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            var result = _controller.GetCurrentUserOrThrowPublic();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(_testUser.Id, result.Id);
            Assert.AreEqual(_testUser.Email, result.Email);
        }

        [TestMethod]
        public void GetCurrentUserOrThrow_WhenUserNotExists_ShouldThrowUnauthorizedAccessException()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act & Assert
            var exception = Assert.ThrowsException<UnauthorizedAccessException>(() =>
                _controller.GetCurrentUserOrThrowPublic());

            Assert.AreEqual("Current user not found in request context", exception.Message);
        }
    }

    // Test controller to expose protected methods
    public class TestAuthController : AuthBaseController
    {
        public User? GetCurrentUser() => CurrentUser;
        public int GetCurrentUserIdPublic() => GetCurrentUserId();
        public string? GetCurrentUserEmailPublic() => GetCurrentUserEmail();
        public ActionResult<User> ValidateCurrentUserPublic() => ValidateCurrentUser();
        public User? GetCurrentUserOrThrowPublic() => GetCurrentUserOrThrow();
    }
}
using ManiaDeLimpeza.Api.Auth;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests
{
    [TestClass]
    public class TokenServiceTests
    {
        private TokenService _tokenService = null!;
        private AuthOptions _authOptions = null!;

        [TestInitialize]
        public void Setup()
        {
            _authOptions = new AuthOptions
            {
                JwtSecret = "this-is-a-very-secure-secret-key-for-testing-purposes-only-123456789",
                ExpireTimeInSeconds = 3600 // 1 hour
            };

            var options = Options.Create(_authOptions);
            _tokenService = new TokenService(options);
        }

        [TestMethod]
        public void GenerateToken_ShouldReturnValidJwtToken()
        {
            // Arrange
            var userId = "123";
            var email = "test@example.com";

            // Act
            var token = _tokenService.GenerateToken(userId, email);

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
            
            // Verify it's a valid JWT structure (has 3 parts separated by dots)
            var parts = token.Split('.');
            Assert.AreEqual(3, parts.Length);
        }

        [TestMethod]
        public void GenerateToken_ShouldContainCorrectClaims()
        {
            // Arrange
            var userId = "123";
            var email = "test@example.com";

            // Act
            var token = _tokenService.GenerateToken(userId, email);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);

            // JWT tokens use short form claim names
            var userIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "nameid");
            var emailClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "email");

            Assert.IsNotNull(userIdClaim, "UserID claim should not be null");
            Assert.IsNotNull(emailClaim, "Email claim should not be null");
            Assert.AreEqual(userId, userIdClaim.Value);
            Assert.AreEqual(email, emailClaim.Value);
        }

        [TestMethod]
        public void GenerateToken_ShouldSetCorrectExpiration()
        {
            // Arrange
            var userId = "123";
            var email = "test@example.com";
            var beforeGeneration = DateTime.UtcNow;

            // Act
            var token = _tokenService.GenerateToken(userId, email);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);

            var expectedExpiration = beforeGeneration.AddSeconds(_authOptions.ExpireTimeInSeconds);
            var actualExpiration = jsonToken.ValidTo;

            // Allow for a small time difference (5 seconds) due to execution time
            var timeDifference = Math.Abs((expectedExpiration - actualExpiration).TotalSeconds);
            Assert.IsTrue(timeDifference < 5, $"Expected expiration around {expectedExpiration}, but got {actualExpiration}");
        }

        [TestMethod]
        public void GenerateToken_WithDifferentInputs_ShouldGenerateDifferentTokens()
        {
            // Arrange
            var userId1 = "123";
            var email1 = "test1@example.com";
            var userId2 = "456";
            var email2 = "test2@example.com";

            // Act
            var token1 = _tokenService.GenerateToken(userId1, email1);
            var token2 = _tokenService.GenerateToken(userId2, email2);

            // Assert
            Assert.AreNotEqual(token1, token2);
        }

        [TestMethod]
        public void GenerateToken_WithEmptyUserId_ShouldStillGenerateToken()
        {
            // Arrange
            var userId = "";
            var email = "test@example.com";

            // Act
            var token = _tokenService.GenerateToken(userId, email);

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
            
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var userIdClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "nameid");
            
            Assert.IsNotNull(userIdClaim);
            Assert.AreEqual(string.Empty, userIdClaim.Value);
        }

        [TestMethod]
        public void GenerateToken_WithEmptyEmail_ShouldStillGenerateToken()
        {
            // Arrange
            var userId = "123";
            var email = "";

            // Act
            var token = _tokenService.GenerateToken(userId, email);

            // Assert
            Assert.IsFalse(string.IsNullOrWhiteSpace(token));
            
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var emailClaim = jsonToken.Claims.FirstOrDefault(x => x.Type == "email");
            
            Assert.IsNotNull(emailClaim);
            Assert.AreEqual(string.Empty, emailClaim.Value);
        }

        [TestMethod]
        public void GenerateToken_ShouldUseHmacSha256Algorithm()
        {
            // Arrange
            var userId = "123";
            var email = "test@example.com";

            // Act
            var token = _tokenService.GenerateToken(userId, email);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);

            Assert.AreEqual("HS256", jsonToken.Header.Alg);
        }
    }
}
using ManiaDeLimpeza.Application.Services;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.Exceptions;
using ManiaDeLimpeza.Infrastructure.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.UnitTests.Services
{
    [TestClass]
    public class UserServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private UserService _userService = null!;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [TestMethod]
        public async Task CreateUserAsync_ShouldHashPassword_AndCallAdd()
        {
            // Arrange
            var user = new User
            {
                Name = "Test",
                Email = "test@example.com",
                Password = "plaintext"
            };

            // Act
            var result = await _userService.CreateUserAsync(user);

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddAsync(It.Is<User>(u =>
                u.Email == "test@example.com" &&
                u.Password != "plaintext" &&
                u.Password.StartsWith("AQAAAA") // identity hash format
            )), Times.Once);
        }

        [TestMethod]
        public async Task GetByCredentialsAsync_WhenPasswordMatchesAndUserIsActive_ShouldNotReturnUser()
        {
            // Arrange
            var password = "secret";
            var user = new User { Email = "test@example.com", Password = PasswordHelper.Hash(password, new User()), IsActive = true };

            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByCredentialsAsync(user.Email, password);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(user.Email, result!.Email);
        }

        [TestMethod]
        public async Task GetByCredentialsAsync_WhenPasswordMatchesAndUserIsInactive_ShouldNotReturnUser()
        {
            // Arrange
            var password = "secret";
            var user = new User { Email = "test@example.com", Password = PasswordHelper.Hash(password, new User()), IsActive = false };

            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            // Act & Assert
            var ex = await Assert.ThrowsExceptionAsync<BusinessException>(async () =>
            {
                await _userService.GetByCredentialsAsync(user.Email, password);
            });
        }


        [TestMethod]
        public async Task GetByCredentialsAsync_ShouldReturnNull_WhenPasswordDoesNotMatch()
        {
            // Arrange
            var user = new User { Email = "test@example.com", Password = PasswordHelper.Hash("secret", new User()) };

            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByCredentialsAsync(user.Email, "wrongpassword");

            // Assert
            Assert.IsNull(result);
        }
    }
}

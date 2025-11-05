using ManiaDeLimpeza.Application.Interfaces;
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
        private Mock<ICompanyServices> _companyServicesMock = null!;
        private Mock<IUserRepository> _userRepositoryMock = null!;
        private UserService _userService = null!;

        [TestInitialize]
        public void Setup()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _companyServicesMock = new Mock<ICompanyServices>();

            _userService = new UserService(
                _userRepositoryMock.Object,
                _companyServicesMock.Object,
                _companyServicesMock.Object as AutoMapper.IMapper
            );
        }

        [TestMethod]
        public async Task CreateUserAsync_ShouldHashPassword_AndCallAdd()
        {
            // Arrange
            var user = new User
            {
                Name = "Test",
                Email = "test@example.com",
                PasswordHash = "plaintext"
            };

            var company = new Company
            {
                Name = "Test Company",
                
            };

            _userRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                               .ReturnsAsync((User u) => { u.Id = 1; return u; });

            // Act
            var result = await _userService.CreateUserAsync(user, "plaintext");  

            // Assert
            _userRepositoryMock.Verify(repo => repo.AddAsync(It.Is<User>(u =>
                u.Email == "test@example.com" &&
                u.PasswordHash != "plaintext" &&
                u.PasswordHash.StartsWith("AQAAAA") 
            )), Times.Once);
        }

        [TestMethod]
        public async Task GetByCredentialsAsync_ShouldReturnNull_WhenPasswordDoesNotMatch()
        {
            // Arrange
            var user = new User { Email = "test@example.com", PasswordHash = PasswordHelper.Hash("secret", new User()) };

            _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            // Act
            var result = await _userService.GetByCredentialsAsync(user.Email, "wrongpassword");

            // Assert
            Assert.IsNull(result);
        }
    }
}

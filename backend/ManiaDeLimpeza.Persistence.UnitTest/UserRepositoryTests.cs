using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Persistence;
using ManiaDeLimpeza.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Persistence.UnitTest
{
    [TestClass]
    public class UserRepositoryTests
    {
        private ApplicationDbContext CreateInMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        [TestMethod]
        public async Task AddUser_ShouldPersistUserInDatabase()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var repository = new UserRepository(context);

            var user = new User
            {
                Name = "Test User",
                Email = "test@example.com",
                PasswordHash = "123456"
            };

            // Act
            await repository.AddAsync(user);
            var users = await repository.GetAllAsync();

            // Assert
            Assert.AreEqual(1, users.Count());
            Assert.AreEqual("test@example.com", users.First().Email);
        }


        [TestMethod]
        public async Task AddUser_DuplicatedUser_ShouldPersistOnlyOneUserInDatabase()
        {
            // Arrange
            var context = CreateInMemoryContext();
            var repository = new UserRepository(context);

            var userA = new User
            {
                Name = "Test User A",
                Email = "test@example.com",
                PasswordHash = "123456"
            };


            var userB = new User
            {
                Name = "Test User B",
                Email = "test@example.com",
                PasswordHash = "12345678"
            };

            // Act
            await repository.AddAsync(userA);

            Exception exception = null;

            try
            {
                await repository.AddAsync(userB);
            }
            catch (Exception ex)
            {
                exception = ex;
            }


            var users = await repository.GetAllAsync();

            // Assert
            Assert.AreEqual(1, users.Count());
            Assert.AreEqual("test@example.com", users.First().Email);
            Assert.IsNotNull(exception);
        }
    }
}
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Persistence;
using ManiaDeLimpeza.Persistence.IntegrationTests.Tools;
using ManiaDeLimpeza.Persistence.Repositories;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Persistence.IntegrationTests
{
    [TestClass]
    public class UserRepositoryIntegrationTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            TestDbContextFactory.InitializeConfiguration();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            using var context = TestDbContextFactory.CreateContext();
            await context.Database.ExecuteSqlRawAsync("DELETE FROM [Users]");
        }

        [TestMethod]
        public async Task AddUser_ShouldSucceed()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new UserRepository(context);

            var user1 = new User { Name = "Test A", Email = "a@example.com", Password = "123" };

            await repo.AddAsync(user1);

            var all = await repo.GetAllAsync();

            Assert.AreEqual(1, all.Count());
            Assert.AreEqual(user1.Email, all.First().Email);
        }

        [TestMethod]
        [ExpectedException(typeof(DbUpdateException))]
        public async Task AddUser_WithDuplicateEmail_ShouldThrowException()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new UserRepository(context);

            var user1 = new User { Name = "Test A", Email = "a@example.com", Password = "123" };
            var user2 = new User { Name = "Test B", Email = "a@example.com", Password = "456" };

            await repo.AddAsync(user1);
            await repo.AddAsync(user2); // should fail due to unique constraint
        }                


    }
}
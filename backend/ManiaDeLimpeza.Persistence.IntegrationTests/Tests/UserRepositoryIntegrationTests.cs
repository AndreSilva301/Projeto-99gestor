using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Persistence.IntegrationTests.Tools;
using ManiaDeLimpeza.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Persistence.IntegrationTests.Tests
{
    [TestClass]
    public class UserRepositoryIntegrationTests
    {
        private const string SampleEmail = "autogen@example.com";

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
            await context.Database.ExecuteSqlRawAsync("DELETE FROM [Companies]");
        }

        private async Task<Company> CreateTestCompanyAsync(ApplicationDbContext context)
        {
            var company = new Company
            {
                Name = "Test Company",
                CNPJ = "12345678000199"
            };
            
            context.Companies.Add(company);
            await context.SaveChangesAsync();
            return company;
        }

        [TestMethod]
        public async Task AddUser_ShouldSucceed()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new UserRepository(context);
            var company = await CreateTestCompanyAsync(context);

            var user1 = new User 
            { 
                Name = "Test A", 
                Email = "a@example.com", 
                PasswordHash = "123",
                CompanyId = company.Id,
                Profile = UserProfile.Admin
            };

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
            var company = await CreateTestCompanyAsync(context);

            var user1 = new User 
            { 
                Name = "Test A", 
                Email = "a@example.com", 
                PasswordHash = "123",
                CompanyId = company.Id,
                Profile = UserProfile.Admin
            };
            var user2 = new User 
            { 
                Name = "Test B", 
                Email = "a@example.com", 
                PasswordHash = "456",
                CompanyId = company.Id,
                Profile = UserProfile.Admin
            };

            await repo.AddAsync(user1);
            await repo.AddAsync(user2); // should fail due to unique constraint
        }

        [TestMethod]
        public async Task AddUser_ShouldGenerateId()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new UserRepository(context);
            var company = await CreateTestCompanyAsync(context);

            var user = new User
            {
                Name = "Auto-ID Test",
                Email = "autogen@example.com",
                PasswordHash = "secret",
                CompanyId = company.Id,
                Profile = UserProfile.Admin
            };

            await repo.AddAsync(user);

            Assert.IsTrue(user.Id > 0, "Expected User.Id to be set by the database.");
        }

        [TestMethod]
        public async Task GetByEmailAsync_SuccessfullyRetrieveUser()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new UserRepository(context);
            var company = await CreateTestCompanyAsync(context);

            var user = new User
            {
                Name = "Auto-ID Test",
                Email = SampleEmail,
                PasswordHash = "secret",
                CompanyId = company.Id,
                Profile = UserProfile.Admin
            };

            await repo.AddAsync(user);

            var repoUser = await repo.GetByEmailAsync(SampleEmail);

            Assert.IsNotNull(repoUser);
            Assert.AreEqual(user.Name, repoUser.Name);
            Assert.AreEqual(user.Email, repoUser.Email);
            Assert.AreEqual(user.PasswordHash, repoUser.PasswordHash);
        }

        [TestMethod]
        public async Task GetByEmailAsync_FailToRetrieveUser()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new UserRepository(context);
            var company = await CreateTestCompanyAsync(context);

            var user = new User
            {
                Name = "Auto-ID Test",
                Email = SampleEmail,
                PasswordHash = "secret",
                CompanyId = company.Id,
                Profile = UserProfile.Admin
            };

            await repo.AddAsync(user);

            var repoUser = await repo.GetByEmailAsync("non-existent-email@email.com");

            Assert.IsNull(repoUser);
        }

        [TestMethod]
        public async Task UpdateAsync_SuccessfullyUpdate()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new UserRepository(context);
            var company = await CreateTestCompanyAsync(context);

            var user = new User
            {
                Name = "Auto-ID Test",
                Email = SampleEmail,
                PasswordHash = "secret",
                CompanyId = company.Id,
                Profile = UserProfile.Admin
            };

            await repo.AddAsync(user);

            const string differentName = "Different Name";
            user.Name = differentName;

            await repo.UpdateAsync(user);

            var repoUser = await repo.GetByEmailAsync(SampleEmail);

            Assert.IsNotNull(repoUser);
            Assert.AreEqual(differentName, repoUser.Name);
            Assert.AreEqual(user.Email, repoUser.Email);
            Assert.AreEqual(user.PasswordHash, repoUser.PasswordHash);
        }

        [TestMethod]
        public async Task UpdateAsync_PasswordIsntProvided_PasswordIsNotUpdated()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new UserRepository(context);
            var company = await CreateTestCompanyAsync(context);

            var originalUser = new User
            {
                Name = "Original",
                Email = "nopass@example.com",
                PasswordHash = "original123",
                CompanyId = company.Id,
                Profile = UserProfile.Admin
            };

            await repo.AddAsync(originalUser);
            var originalPassword = originalUser.PasswordHash;

            // Simulate update without password
            var updateUser = new User
            {
                Id = originalUser.Id,
                Name = "Updated Name",
                Email = "updated@example.com",
                PasswordHash = "", // should NOT overwrite password
                CompanyId = company.Id,
                Profile = UserProfile.Admin
            };

            await repo.UpdateAsync(updateUser);

            var updatedUser = await repo.GetByEmailAsync("updated@example.com");

            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("Updated Name", updatedUser!.Name);
            Assert.AreEqual(originalPassword, updatedUser.PasswordHash, "Password should not be updated");
        }

        [TestMethod]
        public async Task UpdateAsync_PasswordIsProvided_PasswordIsUpdated()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new UserRepository(context);
            var company = await CreateTestCompanyAsync(context);

            var originalUser = new User
            {
                Name = "Original",
                Email = "withpass@example.com",
                PasswordHash = "original123",
                CompanyId = company.Id,
                Profile = UserProfile.Admin
            };

            await repo.AddAsync(originalUser);

            var newPassword = "new456";
            var updateUser = new User
            {
                Id = originalUser.Id,
                Name = "Updated Name",
                Email = "withpass@example.com",
                PasswordHash = newPassword, // should overwrite password
                CompanyId = company.Id,
                Profile = UserProfile.Admin
            };

            await repo.UpdateAsync(updateUser);

            var updatedUser = await repo.GetByEmailAsync("withpass@example.com");

            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("Updated Name", updatedUser!.Name);
            Assert.AreEqual(newPassword, updatedUser.PasswordHash, "Password should be updated");
        }
    }
}
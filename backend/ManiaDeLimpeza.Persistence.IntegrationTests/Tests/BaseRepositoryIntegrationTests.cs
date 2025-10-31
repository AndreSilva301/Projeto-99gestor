using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Persistence.IntegrationTests.Tools;
using ManiaDeLimpeza.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Persistence.IntegrationTests.Tests
{
    [TestClass]
    public class BaseRepositoryIntegrationTests
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
            // Fix table name - should be Customers not Clients
            await context.Database.ExecuteSqlRawAsync("DELETE FROM [Customers]");
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
        public async Task Add_ShouldGenerateId()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new BaseRepositoryImpl<Customer>(context);
            var company = await CreateTestCompanyAsync(context);

            var client = new Customer
            {
                Name = "Maria",
                Email = "maria@example.com",
                CompanyId = company.Id // Add required CompanyId
            };

            await repo.AddAsync(client);

            Assert.IsTrue(client.Id > 0);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnClient()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new BaseRepositoryImpl<Customer>(context);
            var company = await CreateTestCompanyAsync(context);

            var client = new Customer
            {
                Name = "João",
                Email = "joao@example.com",
                CompanyId = company.Id // Add required CompanyId
            };

            await repo.AddAsync(client);
            var fromDb = await repo.GetByIdAsync(client.Id);

            Assert.IsNotNull(fromDb);
            Assert.AreEqual(client.Email, fromDb!.Email);
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnAllClients()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new BaseRepositoryImpl<Customer>(context);
            var company = await CreateTestCompanyAsync(context);

            await repo.AddAsync(new Customer { Name = "A", Email = "a@example.com", CompanyId = company.Id });
            await repo.AddAsync(new Customer { Name = "B", Email = "b@example.com", CompanyId = company.Id });

            var all = await repo.GetAllAsync();

            Assert.AreEqual(2, all.Count());
        }

        [TestMethod]
        public async Task Update_ShouldPersistChanges()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new BaseRepositoryImpl<Customer>(context);
            var company = await CreateTestCompanyAsync(context);

            var client = new Customer
            {
                Name = "Lucas",
                Email = "lucas@example.com",
                CompanyId = company.Id // Add required CompanyId
            };

            await repo.AddAsync(client);

            client.Name = "Lucas Silva";
            await repo.UpdateAsync(client);

            var updated = await repo.GetByIdAsync(client.Id);
            Assert.AreEqual("Lucas Silva", updated!.Name);
        }

        [TestMethod]
        public async Task Delete_ShouldRemoveClient()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new BaseRepositoryImpl<Customer>(context);
            var company = await CreateTestCompanyAsync(context);

            var client = new Customer
            {
                Name = "To Remove",
                Email = "remove@example.com",
                CompanyId = company.Id // Add required CompanyId
            };

            await repo.AddAsync(client);
            await repo.DeleteAsync(client);

            var deleted = await repo.GetByIdAsync(client.Id);
            Assert.IsNull(deleted);
        }

        [TestMethod]
        public async Task Client_FullLifecycle_WithOwnedAddressAndPhone_ShouldSucceed()
        {
            using var context = TestDbContextFactory.CreateContext();
            var clientRepo = new BaseRepositoryImpl<Customer>(context);
            var company = await CreateTestCompanyAsync(context);

            // 1. Add client with full data
            var client = new Customer
            {
                Name = "Ana Silva",
                Email = "ana@example.com",
                CreatedDate = new DateTime(1990, 5, 21),
                Observations = "VIP client",
                CompanyId = company.Id, // Add required CompanyId
                Address = new Address
                {
                    Street = "123 Main St",
                    Number = "456",
                    Complement = "Apt 7",
                    Neighborhood = "Downtown",
                    City = "Cityville",
                    State = "CA",
                    ZipCode = "12345-678"
                },
                Phone = new Phone
                {
                    Mobile = "(11) 99999-8888",
                    Landline = "(11) 2222-3333"
                }
            };

            await clientRepo.AddAsync(client);

            // 2. Read
            var loaded = await clientRepo.GetByIdAsync(client.Id);
            Assert.IsNotNull(loaded);
            Assert.AreEqual("Ana Silva", loaded!.Name);
            Assert.AreEqual("123 Main St", loaded.Address.Street);
            Assert.AreEqual("(11) 99999-8888", loaded.Phone.Mobile);

            // 3. Update
            loaded.Name = "Ana Carolina Silva";
            loaded.Address.City = "UpdatedCity";
            loaded.Phone.Mobile = "(99) 11111-2222";

            await clientRepo.UpdateAsync(loaded);

            var updated = await clientRepo.GetByIdAsync(client.Id);
            Assert.AreEqual("Ana Carolina Silva", updated!.Name);
            Assert.AreEqual("UpdatedCity", updated.Address.City);
            Assert.AreEqual("(99) 11111-2222", updated.Phone.Mobile);

            // 4. Delete
            await clientRepo.DeleteAsync(updated);
            var deleted = await clientRepo.GetByIdAsync(client.Id);
            Assert.IsNull(deleted);
        }

        public class BaseRepositoryImpl<T> : BaseRepository<T> where T : class
        {
            public BaseRepositoryImpl(ApplicationDbContext context) : base(context)
            {

            }
        }
    }
}

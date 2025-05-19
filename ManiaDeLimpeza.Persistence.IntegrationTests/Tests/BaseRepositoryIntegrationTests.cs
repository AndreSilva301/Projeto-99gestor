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
            await context.Database.ExecuteSqlRawAsync("DELETE FROM [Clients]");
        }

        [TestMethod]
        public async Task Add_ShouldGenerateId()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new BaseRepositoryImpl<Client>(context);

            var client = new Client
            {
                Name = "Maria",
                Email = "maria@example.com"
            };

            await repo.AddAsync(client);

            Assert.IsTrue(client.Id > 0);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnClient()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new BaseRepositoryImpl<Client>(context);

            var client = new Client
            {
                Name = "João",
                Email = "joao@example.com"
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
            var repo = new BaseRepositoryImpl<Client>(context);

            await repo.AddAsync(new Client { Name = "A", Email = "a@example.com" });
            await repo.AddAsync(new Client { Name = "B", Email = "b@example.com" });

            var all = await repo.GetAllAsync();

            Assert.AreEqual(2, all.Count());
        }

        [TestMethod]
        public async Task Update_ShouldPersistChanges()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new BaseRepositoryImpl<Client>(context);

            var client = new Client
            {
                Name = "Lucas",
                Email = "lucas@example.com"
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
            var repo = new BaseRepositoryImpl<Client>(context);

            var client = new Client
            {
                Name = "To Remove",
                Email = "remove@example.com"
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
            var clientRepo = new BaseRepositoryImpl<Client>(context);

            // 1. Add client with full data
            var client = new Client
            {
                Name = "Ana Silva",
                Email = "ana@example.com",
                Birthday = new DateTime(1990, 5, 21),
                Observations = "VIP client",
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

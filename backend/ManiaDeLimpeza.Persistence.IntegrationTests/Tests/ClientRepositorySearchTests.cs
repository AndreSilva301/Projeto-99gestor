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
    public class ClientRepositorySearchTests
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

        private static Customer CreateClient(string name, string mobile = "", string landline = "")
        {
            return new Customer
            {
                Name = name,
                Email = $"{name.Replace(" ", "").ToLower()}@example.com",
                Address = new Address
                {
                    Street = "Test St",
                    Number = "1",
                    City = "Testville",
                    Neighborhood = "Central",
                    State = "TS",
                    ZipCode = "12345"
                },
                Phone = new Phone
                {
                    Mobile = mobile,
                    Landline = landline
                }
            };
        }

        [TestMethod]
        public async Task SearchAsync_ShouldMatchByName()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new ClientRepository(context);

            var client = CreateClient("Ana Maria");
            await repo.AddAsync(client);

            var result = await repo.SearchAsync("ana");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Ana Maria", result[0].Name);
        }

        [TestMethod]
        public async Task SearchAsync_ShouldMatchByMobile()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new ClientRepository(context);

            var client = CreateClient("Test User", mobile: "99999-0000");
            await repo.AddAsync(client);

            var result = await repo.SearchAsync("99999");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test User", result[0].Name);
        }

        [TestMethod]
        public async Task SearchAsync_ShouldMatchByLandline()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new ClientRepository(context);

            var client = CreateClient("Test User", landline: "2222-3333");
            await repo.AddAsync(client);

            var result = await repo.SearchAsync("2222");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Test User", result[0].Name);
        }

        [TestMethod]
        public async Task SearchAsync_ShouldMatchMultipleFieldsAndRankHigher()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new ClientRepository(context);

            var client = CreateClient("Ana Silva", mobile: "99999-1234");
            await repo.AddAsync(client);

            var result = await repo.SearchAsync("ana 9999");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Ana Silva", result[0].Name);
        }

        [TestMethod]
        public async Task SearchAsync_ShouldReturnBestMatchFirst()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new ClientRepository(context);

            var best = CreateClient("João Pedro", landline: "4321");
            var weaker = CreateClient("Maria João");

            await repo.AddAsync(best);
            await repo.AddAsync(weaker);

            var result = await repo.SearchAsync("joão 4321");

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("João Pedro", result[0].Name); // best match first
        }

        [TestMethod]
        public async Task SearchAsync_ShouldReturnEmptyWhenNoMatch()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new ClientRepository(context);

            await repo.AddAsync(CreateClient("Someone"));

            var result = await repo.SearchAsync("notfound");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public async Task SearchAsync_ShouldReturnAllWhenInputIsBlank()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new ClientRepository(context);

            await repo.AddAsync(CreateClient("Blank Test"));

            var result = await repo.SearchAsync("  ");

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public async Task SearchAsync_ShouldBeCaseInsensitive()
        {
            using var context = TestDbContextFactory.CreateContext();
            var repo = new ClientRepository(context);

            await repo.AddAsync(CreateClient("ana silva"));

            var result = await repo.SearchAsync("ANA");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("ana silva", result[0].Name);
        }
    }
}

using ManiaDeLimpeza.Api.IntegrationTests.Tools;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain;
using ManiaDeLimpeza.Persistence;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests
{
    [TestClass]
    public class QuoteSearchIntegrationTests
    {
        private static CustomWebApplicationFactory _factory;
        private static HttpClient _client;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TestInitialize]
        public async Task TestInit()
        {
            await AuthenticateAsync();
            await SeedQuotesAndClientsAsync();
        }

        [TestCleanup]
        public void Cleanup()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            TestDataCleanup.ClearUsers(db);
            TestDataCleanup.ClearClients(db);
            TestDataCleanup.ClearQuotes(db);
        }

        private static async Task AuthenticateAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            TestDataSeeder.SeedDefaultUser(db);

            var login = new { Email = "testuser@example.com", Password = TestDataSeeder.DefaultPassword };
            var response = await _client.PostAsync("/api/auth/login",
                new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json"));

            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<AuthResponseDto>(json);
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", obj!.BearerToken);
        }

        private static async Task SeedQuotesAndClientsAsync()
        {
            for (int i = 1; i <= 10; i++)
            {
                var client = new Customer
                {
                    Name = i switch
                    {
                        1 => "Maria José",
                        2 => "João Silva",
                        3 => "Ana Souza",
                        4 => "Carlos Lima",
                        5 => "Pedro Martins",
                        6 => "Juliana Paes",
                        7 => "Lucas Rocha",
                        8 => "Bruna Costa",
                        9 => "José Almeida",
                        _ => "Fernanda Dias"
                    },
                    Email = $"client{i}@example.com",
                    Phone = new Phone { Mobile = $"99999-00{i:D2}" }
                };

                var clientResponse = await _client.PostAsync("/api/client",
                    new StringContent(JsonConvert.SerializeObject(client), Encoding.UTF8, "application/json"));
                var createdClient = JsonConvert.DeserializeObject<Customer>(await clientResponse.Content.ReadAsStringAsync());

                for (int j = 0; j < (i <= 5 ? 3 : 2); j++) // total 25 quotes
                {
                    var quote = new QuoteDto
                    {
                        ClientId = createdClient!.Id,
                        PaymentMethod = PaymentConditions.Pix,
                        CashDiscount = 0,
                        LineItems = new List<LineItemDto>
                        {
                            new LineItemDto
                            {
                                Description = $"Service {i}-{j}",
                                Quantity = 1,
                                UnitPrice = 100,
                                Total = 100
                            }
                        }
                    };
                    await _client.PostAsync("/api/quote",
                        new StringContent(JsonConvert.SerializeObject(quote), Encoding.UTF8, "application/json"));
                }
            }
        }

        private async Task<PagedResult<Quote>> PostSearchAsync(QuoteFilterDto filter)
        {
            var response = await _client.PostAsync("/api/quote/search",
                new StringContent(JsonConvert.SerializeObject(filter), Encoding.UTF8, "application/json"));
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            return JsonConvert.DeserializeObject<PagedResult<Quote>>(await response.Content.ReadAsStringAsync())!;
        }
        [TestMethod]
        public async Task SearchByClientName_ShouldReturnMatches()
        {
            var result = await PostSearchAsync(new() { ClientName = "maria" });
            Assert.IsTrue(result.Items.All(q => q.Customer.Name.Contains("Maria")), "Expected all results to contain 'Maria' in client name");
        }

        [TestMethod]
        public async Task SearchByClientPhone_ShouldReturnMatches()
        {
            var result = await PostSearchAsync(new() { ClientPhone = "001" });
            Assert.IsTrue(result.Items.All(q => q.Customer.Phone.Mobile.Contains("001")), "Expected all results to contain '001' in mobile phone");
        }

        [TestMethod]
        public async Task CombinedFilters_ShouldReturnCorrectResults()
        {
            var result = await PostSearchAsync(new() { ClientName = "João", ClientPhone = "002" });
            Assert.IsTrue(result.Items.All(q =>
                q.Customer.Name.Contains("João") && q.Customer.Phone.Mobile.Contains("002")),
                "Expected all results to match both name 'João' and phone '002'");
        }

        [TestMethod]
        public async Task NoMatchFilter_ShouldReturnZero()
        {
            var result = await PostSearchAsync(new() { ClientName = "XYZ" });
            Assert.AreEqual(0, result.TotalItems, "Expected no results for non-matching client name");
        }

        [TestMethod]
        public async Task LastPage_ShouldReturnFewerItems()
        {
            var result = await PostSearchAsync(new() { Page = 3, PageSize = 10 });
            Assert.IsTrue(result.Items.Count < 10, "Expected last page to contain fewer than 10 items");
        }

        [TestMethod]
        public async Task OutOfBoundsPage_ShouldReturnEmpty()
        {
            var result = await PostSearchAsync(new() { Page = 99, PageSize = 10 });
            Assert.AreEqual(0, result.Items.Count, "Expected no items on out-of-bounds page");
        }

        [TestMethod]
        public async Task SortByClientNameAscending_ShouldWork()
        {
            var result = await PostSearchAsync(new() { SortBy = "ClientName", SortDescending = false });
            var expectedOrder = result.Items.Select(x => x).OrderBy(q => q.Customer.Name).ToList();
            CollectionAssert.AreEqual(expectedOrder, result.Items.ToList(), "Expected items to be sorted by ClientName ascending");
        }

        [TestMethod]
        public async Task SortByClientNameDescending_ShouldWork()
        {
            var result = await PostSearchAsync(new() { SortBy = "ClientName", SortDescending = true });
            var expectedOrder = result.Items.OrderByDescending(q => q.Customer.Name).ToList();
            CollectionAssert.AreEqual(expectedOrder, result.Items.ToList(), "Expected items to be sorted by ClientName descending");
        }

        [TestMethod]
        public async Task SortByCreatedAtDescending_ShouldWork()
        {
            var result = await PostSearchAsync(new() { SortBy = "CreatedAt", SortDescending = true });
            var expectedOrder = result.Items.OrderByDescending(q => q.CreatedAt).ToList();
            CollectionAssert.AreEqual(expectedOrder, result.Items.ToList(), "Expected items to be sorted by CreatedAt descending");
        }

        [TestMethod]
        public async Task EmptyFilter_ShouldReturnAllPaged()
        {
            var result = await PostSearchAsync(new() { Page = 1, PageSize = 10 });
            Assert.IsTrue(result.Items.Count > 0, "Expected at least one item returned for empty filter");
        }

        [TestMethod]
        public async Task AccentInsensitiveSearch_ShouldReturnMatches()
        {
            var result = await PostSearchAsync(new() { ClientName = "jose" });
            Assert.IsTrue(result.Items.Any(q => q.Customer.Name.Contains("José")),
                "Expected at least one result to match 'José' when searching for 'jose'");
        }

    }
}

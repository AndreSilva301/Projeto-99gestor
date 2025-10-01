using ManiaDeLimpeza.Api.IntegrationTests.Tools;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain;
using ManiaDeLimpeza.Persistence;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests
{
    [TestClass]
    public class QuoteControllerIntegrationTests
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
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", obj!.BearerToken);
        }

        [TestMethod]
        public async Task CreateQuote_ShouldSucceed()
        {
            var client = await CreateTestClient();

            var quote = new QuoteDto
            {
                ClientId = client.Id,
                CashDiscount = 10,
                PaymentMethod = PaymentMethod.Pix,
                LineItems = new List<LineItemDto>
                {
                    new LineItemDto { Description = "Cleaning", Quantity = 1, UnitPrice = 100, Total = 100 }
                }
            };

            var response = await _client.PostAsync("/api/quote",
                new StringContent(JsonConvert.SerializeObject(quote), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            var created = JsonConvert.DeserializeObject<QuoteDto>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(90, created!.TotalPrice); // 100 - 10
        }

        [TestMethod]
        public async Task GetQuoteById_ShouldReturnQuote()
        {
            var quote = await CreateTestQuote();
            var response = await _client.GetAsync($"/api/quote/{quote.Id}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var fetched = JsonConvert.DeserializeObject<QuoteDto>(await response.Content.ReadAsStringAsync());
            Assert.AreEqual(quote.Id, fetched!.Id);
        }

        [TestMethod]
        public async Task UpdateQuote_ShouldSucceed()
        {
            var quote = await CreateTestQuote();
            quote.CashDiscount = 50;

            var response = await _client.PutAsync($"/api/quote/{quote.Id}",
                new StringContent(JsonConvert.SerializeObject(quote), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

            var check = await _client.GetAsync($"/api/quote/{quote.Id}");
            var updated = JsonConvert.DeserializeObject<QuoteDto>(await check.Content.ReadAsStringAsync());
            Assert.AreEqual(50, updated!.CashDiscount);
        }

        [TestMethod]
        public async Task ArchiveQuote_ShouldSucceed()
        {
            var quote = await CreateTestQuote();
            var response = await _client.DeleteAsync($"/api/quote/{quote.Id}");
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

            var check = await _client.GetAsync($"/api/quote/{quote.Id}");
            Assert.AreEqual(HttpStatusCode.NotFound, check.StatusCode);
        }

        [TestMethod]
        public async Task UpdateQuote_ShouldRemoveLineItems()
        {
            // Step 1: Create a client
            var client = await CreateTestClient();

            // Step 2: Create a quote with 3 line items
            var quote = new QuoteDto
            {
                ClientId = client!.Id,
                CashDiscount = 0,
                PaymentMethod = PaymentMethod.DebitCard,
                LineItems = new List<LineItemDto>
                {
                    new LineItemDto { Description = "Item 1", Quantity = 1, UnitPrice = 30, Total = 30 },
                    new LineItemDto { Description = "Item 2", Quantity = 2, UnitPrice = 20, Total = 40 },
                    new LineItemDto { Description = "Item 3", Quantity = 1, UnitPrice = 50, Total = 50 }
                }
            };

            var createResponse = await _client.PostAsync("/api/quote",
                new StringContent(JsonConvert.SerializeObject(quote), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.Created, createResponse.StatusCode);
            var createdQuote = JsonConvert.DeserializeObject<QuoteDto>(await createResponse.Content.ReadAsStringAsync())!;
            Assert.AreEqual(3, createdQuote.LineItems.Count);

            // Step 3: Update quote to keep only one line item (e.g., "Item 2")
            var remaining = createdQuote.LineItems.First(li => li.Description == "Item 2");
            createdQuote.LineItems = new List<LineItemDto>
            {
                new LineItemDto
                {
                    Id = remaining.Id, // Include Id for EF to track it as update
                    Description = remaining.Description,
                    Quantity = remaining.Quantity,
                    UnitPrice = remaining.UnitPrice,
                    Total = remaining.Total
                }
            };
            createdQuote.TotalPrice = remaining.Total;

            var updateResponse = await _client.PutAsync($"/api/quote/{createdQuote.Id}",
                new StringContent(JsonConvert.SerializeObject(createdQuote), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.NoContent, updateResponse.StatusCode);

            // Step 4: Fetch and verify only 1 item remains
            var getResponse = await _client.GetAsync($"/api/quote/{createdQuote.Id}");
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var updatedQuote = JsonConvert.DeserializeObject<QuoteDto>(await getResponse.Content.ReadAsStringAsync());
            Assert.AreEqual(1, updatedQuote!.LineItems.Count);
            Assert.AreEqual("Item 2", updatedQuote.LineItems[0].Description);
        }

        [TestMethod]
        public async Task GetQuoteById_ShouldIncludeLineItemsAndClient()
        {
            // Arrange: cria cliente
            var client = await CreateTestClient();

            // Cria um quote com dois itens
            var quote = new QuoteDto
            {
                ClientId = client!.Id,
                CashDiscount = 0,
                PaymentMethod = PaymentMethod.CreditCard,
                LineItems = new List<LineItemDto>
                {
                    new LineItemDto { Description = "Item A", Quantity = 1, UnitPrice = 50, Total = 50 },
                    new LineItemDto { Description = "Item B", Quantity = 2, UnitPrice = 25, Total = 50 }
                }
            };

            var response = await _client.PostAsync("/api/quote",
                new StringContent(JsonConvert.SerializeObject(quote), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var createdQuote = JsonConvert.DeserializeObject<QuoteDto>(await response.Content.ReadAsStringAsync())!;
            Assert.AreEqual(2, createdQuote.LineItems.Count);

            // Act: recupera quote por ID
            var getResponse = await _client.GetAsync($"/api/quote/{createdQuote.Id}");
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var fetched = JsonConvert.DeserializeObject<Quote>(await getResponse.Content.ReadAsStringAsync())!;

            // Assert: line items e client estão preenchidos
            Assert.AreEqual(2, fetched.LineItems.Count, "Expected 2 line items");
            Assert.IsTrue(fetched.LineItems.Any(li => li.Description == "Item A"));
            Assert.IsTrue(fetched.LineItems.Any(li => li.Description == "Item B"));
            Assert.AreEqual(client.Name, fetched.Client?.Name);
            Assert.AreEqual(client.Email, fetched.Client?.Email);
        }


        private async Task<QuoteDto> CreateTestQuote()
        {
            Client? createdClient = await CreateTestClient();

            var quote = new QuoteDto
            {
                ClientId = createdClient!.Id,
                CashDiscount = 0,
                PaymentMethod = PaymentMethod.CreditCard,
                LineItems = new List<LineItemDto>
                {
                    new LineItemDto { Description = "Test Service", Quantity = 2, UnitPrice = 50, Total = 100 }
                }
            };

            var response = await _client.PostAsync("/api/quote",
                new StringContent(JsonConvert.SerializeObject(quote), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            return JsonConvert.DeserializeObject<QuoteDto>(await response.Content.ReadAsStringAsync())!;
        }

        private static async Task<Client?> CreateTestClient()
        {
            var client = new Client
            {
                Name = "Quote Client",
                Email = "quoteclient@example.com"
            };

            var clientResponse = await _client.PostAsync("/api/client",
                new StringContent(JsonConvert.SerializeObject(client), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.Created, clientResponse.StatusCode);

            var createdClient = JsonConvert.DeserializeObject<Client>(await clientResponse.Content.ReadAsStringAsync());
            return createdClient;
        }
    }
}

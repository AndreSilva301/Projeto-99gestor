using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace ManiaDeLimpeza.Api.IntegrationTests
{
    [TestClass]
    public class QuoteItemsControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;
        private IServiceScope _scope = null!;
        private ApplicationDbContext _dbContext = null!;

        [TestInitialize]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                        if (descriptor != null)
                            services.Remove(descriptor);

                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("QuoteItemsTests_" + Guid.NewGuid());
                        });
                    });
                });

            _scope = _factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            _client = _factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            _scope.Dispose();
            _client.Dispose();
            _factory.Dispose();
        }

        [TestMethod]
        public async Task AddItem_ValidData_Returns201Created()
        {
            var quote = new Quote
            {
                CustomerId = 1,
                UserId = 1,
                TotalPrice = 1000,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentConditions = "Pagamento em 2x"
            };
            _dbContext.Quotes.Add(quote);
            await _dbContext.SaveChangesAsync();

            var dto = new CreateQuoteItemDto
            {
                Description = "Limpeza de escritório",
                Quantity = 2,
                UnitPrice = 500
            };

            var response = await _client.PostAsJsonAsync($"/api/quotes/{quote.Id}/items", dto);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateItem_ValidData_Returns200OK()
        {
            var quote = new Quote
            {
                CustomerId = 1,
                UserId = 1,
                TotalPrice = 200,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "À vista"
            };
            _dbContext.Quotes.Add(quote);

            var item = new QuoteItem
            {
                Quote = quote,
                Description = "Serviço inicial",
                Quantity = 1,
                UnitPrice = 200
            };
            _dbContext.QuoteItems.Add(item);
            await _dbContext.SaveChangesAsync();

            var updateDto = new UpdateQuoteItemDto
            {
                Description = "Serviço atualizado",
                Quantity = 2,
                UnitPrice = 150
            };

            var response = await _client.PutAsJsonAsync($"/api/quote-items/{item.Id}", updateDto);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteItem_ExistingItem_Returns204NoContent()
        {
            var quote = new Quote
            {
                CustomerId = 1,
                UserId = 1,
                TotalPrice = 300,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "À vista"
            };
            _dbContext.Quotes.Add(quote);

            var item1 = new QuoteItem { Quote = quote, Description = "Item 1", Quantity = 1, UnitPrice = 150 };
            var item2 = new QuoteItem { Quote = quote, Description = "Item 2", Quantity = 1, UnitPrice = 150 };
            _dbContext.QuoteItems.AddRange(item1, item2);
            await _dbContext.SaveChangesAsync();

            var response = await _client.DeleteAsync($"/api/quote-items/{item1.Id}");
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task ReorderItems_ValidOrder_Returns204NoContent()
        {
            var quote = new Quote
            {
                CustomerId = 1,
                UserId = 1,
                TotalPrice = 400,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "À vista"
            };
            _dbContext.Quotes.Add(quote);

            var item1 = new QuoteItem { Quote = quote, Description = "Item A", Quantity = 1, UnitPrice = 200 };
            var item2 = new QuoteItem { Quote = quote, Description = "Item B", Quantity = 1, UnitPrice = 200 };
            _dbContext.QuoteItems.AddRange(item1, item2);
            await _dbContext.SaveChangesAsync();

            var dto = new { ItemIds = new List<int> { item2.Id, item1.Id } };

            var response = await _client.PostAsJsonAsync($"/api/quotes/{quote.Id}/items/reorder", dto);
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task AddItem_NonExistingQuote_Returns404NotFound()
        {
            var dto = new CreateQuoteItemDto
            {
                Description = "Serviço",
                Quantity = 1,
                UnitPrice = 100
            };

            var response = await _client.PostAsJsonAsync("/api/quotes/999/items", dto);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteItem_LastItem_Returns400BadRequest()
        {
            var quote = new Quote
            {
                CustomerId = 1,
                UserId = 1,
                TotalPrice = 100,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            };
            _dbContext.Quotes.Add(quote);

            var item = new QuoteItem { Quote = quote, Description = "Único item", Quantity = 1, UnitPrice = 100 };
            _dbContext.QuoteItems.Add(item);
            await _dbContext.SaveChangesAsync();

            var response = await _client.DeleteAsync($"/api/quote-items/{item.Id}");
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateItem_NonExistingItem_Returns404NotFound()
        {
            var dto = new UpdateQuoteItemDto
            {
                Description = "Teste",
                Quantity = 1,
                UnitPrice = 50
            };

            var response = await _client.PutAsJsonAsync("/api/quote-items/9999", dto);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task ReorderItems_InvalidItemIds_Returns400BadRequest()
        {
            var quote = new Quote
            {
                CustomerId = 1,
                UserId = 1,
                TotalPrice = 200,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "À vista"
            };
            _dbContext.Quotes.Add(quote);
            await _dbContext.SaveChangesAsync();

            var dto = new { ItemIds = new List<int>() };

            var response = await _client.PostAsJsonAsync($"/api/quotes/{quote.Id}/items/reorder", dto);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

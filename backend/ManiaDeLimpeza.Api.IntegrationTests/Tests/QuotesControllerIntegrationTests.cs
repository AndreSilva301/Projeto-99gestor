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
    public class QuotesControllerIntegrationTests
    {
        private WebApplicationFactory<Program> _factory = null!;
        private HttpClient _client = null!;
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
                            options.UseInMemoryDatabase("TestDb_" + Guid.NewGuid());
                        });
                    });
                });

            _client = _factory.CreateClient();
            _dbContext = _factory.Services.GetRequiredService<ApplicationDbContext>();
        }

        [TestMethod]
        public async Task CreateQuote_ValidData_Returns201Created()
        {
            var dto = new CreateQuoteDto
            {
                CustomerId = 1,
                UserId = 1,
                TotalPrice = 500,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentConditions = "Pagamento em 2x",
                Items = new List<CreateQuoteItemDto>
                {
                    new CreateQuoteItemDto {   Description = "Limpeza Geral", Quantity = 1, UnitPrice = 500 }
                }
            };

            var response = await _client.PostAsJsonAsync("/api/quotes", dto);
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<QuoteResponseDto>();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Id > 0);
        }

        [TestMethod]
        public async Task GetQuotes_WithFilters_Returns200WithFilteredData()
        {
            var customerId = 1;
            _dbContext.Quotes.Add(new Quote { CustomerId = customerId, UserId = 1, TotalPrice = 200, PaymentMethod = PaymentMethod.Cash });
            _dbContext.Quotes.Add(new Quote { CustomerId = 2, UserId = 1, TotalPrice = 300, PaymentMethod = PaymentMethod.Pix });
            await _dbContext.SaveChangesAsync();

            var response = await _client.GetAsync($"/api/quotes?customerId={customerId}");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<List<QuoteResponseDto>>();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.All(q => q.CustomerId == customerId));
        }

        [TestMethod]
        public async Task GetQuoteById_ExistingId_Returns200WithQuote()
        {
            var quote = new Quote
            {
                CustomerId = 1,
                UserId = 1,
                TotalPrice = 250,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "À vista"
            };
            _dbContext.Quotes.Add(quote);
            await _dbContext.SaveChangesAsync();

            var response = await _client.GetAsync($"/api/quotes/{quote.Id}");
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<QuoteResponseDto>();
            Assert.IsNotNull(result);
            Assert.AreEqual(quote.CustomerId, result.CustomerId);
        }

        [TestMethod]
        public async Task UpdateQuote_ValidData_Returns200WithUpdatedQuote()
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
            await _dbContext.SaveChangesAsync();

            var updateDto = new UpdateQuoteDto
            {
                TotalPrice = 350,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentConditions = "Pagamento em 3x"
            };

            var response = await _client.PutAsJsonAsync($"/api/quotes/{quote.Id}", updateDto);
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<QuoteResponseDto>();
            Assert.IsNotNull(result);
            Assert.AreEqual(350, result.TotalPrice);
        }

        [TestMethod]
        public async Task DeleteQuote_ExistingId_Returns204NoContent()
        {
            var quote = new Quote { CustomerId = 1, UserId = 1, TotalPrice = 100, PaymentMethod = PaymentMethod.Cash };
            _dbContext.Quotes.Add(quote);
            await _dbContext.SaveChangesAsync();

            var response = await _client.DeleteAsync($"/api/quotes/{quote.Id}");
            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }

        [TestMethod]
        public async Task CreateQuote_InvalidCustomerId_Returns400BadRequest()
        {
            var dto = new CreateQuoteDto
            {
                CustomerId = 0,
                UserId = 1,
                TotalPrice = 200,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "À vista",
                Items = new List<CreateQuoteItemDto>
                {
                    new CreateQuoteItemDto {   Description = "Serviço X", Quantity = 1, UnitPrice = 200 }
                }
            };

            var response = await _client.PostAsJsonAsync("/api/quotes", dto);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task CreateQuote_EmptyItems_Returns400BadRequest()
        {
            var dto = new CreateQuoteDto
            {
                CustomerId = 1,
                UserId = 1,
                TotalPrice = 200,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "À vista",
                Items = new List<CreateQuoteItemDto>()
            };

            var response = await _client.PostAsJsonAsync("/api/quotes", dto);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task GetQuoteById_NonExistingId_Returns404NotFound()
        {
            var response = await _client.GetAsync("/api/quotes/9999");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateQuote_NonExistingId_Returns404NotFound()
        {
            var updateDto = new UpdateQuoteDto
            {
                TotalPrice = 500,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "À vista"
            };

            var response = await _client.PutAsJsonAsync("/api/quotes/9999", updateDto);
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task DeleteQuote_NonExistingId_Returns404NotFound()
        {
            var response = await _client.DeleteAsync("/api/quotes/9999");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task CreateQuote_InvalidDiscount_Returns400BadRequest()
        {
            var dto = new CreateQuoteDto
            {
                CustomerId = 1,
                UserId = 1,
                TotalPrice = 100,
                CashDiscount = 200,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "À vista",
                Items = new List<CreateQuoteItemDto>
                {
                    new CreateQuoteItemDto {  Description = "Serviço X", Quantity = 1, UnitPrice = 100 }
                }
            };

            var response = await _client.PostAsJsonAsync("/api/quotes", dto);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

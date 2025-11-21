using ManiaDeLimpeza.Api.IntegrationTests.Tools;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Json;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests
{
    [TestClass]
    public class QuotesControllerIntegrationTests
    {
        private static CustomWebApplicationFactory _factory;
        private static HttpClient _client;
        private ApplicationDbContext _db;

        [ClassInitialize]
        public static void Init(TestContext context)
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TestInitialize]
        public void SetupEach()
        {
            var scope = _factory.Services.CreateScope();
            _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        [TestCleanup]
        public void Cleanup()
        {
            TestDataCleanup.ClearQuotes(_db);
            TestDataCleanup.ClearClients(_db);
            TestDataCleanup.ClearUsers(_db);
            TestDataCleanup.ClearCompany(_db);
        }

        private Customer SeedCustomer()
        {
            var company = new Company { Name = "Empresa 1" };
            _db.Companies.Add(company);
            _db.SaveChanges();

            var user = new User
            {
                Email = "user@test.com",
                Name = "User Test",
                PasswordHash = "hash",
                CompanyId = company.Id
            };
            _db.Users.Add(user);
            _db.SaveChanges();

            var customer = new Customer
            {
                Name = "Cliente Teste",
                Phone = new Phone
                {
                    Mobile = "11999999999"
                },
                CompanyId = company.Id
            };
            _db.Customers.Add(customer);
            _db.SaveChanges();

            return customer;
        }

        private Quote SeedQuote()
        {
            var customer = SeedCustomer();
            var quote = new Quote
            {
                CustomerId = customer.Id,
                UserId = _db.Users.First().Id,
                TotalPrice = 300,
                PaymentMethod = PaymentMethod.Pix
            };

            _db.Quotes.Add(quote);
            _db.SaveChanges();

            return quote;
        }

        [TestMethod]
        public async Task CreateQuote_ValidData_ShouldReturn201()
        {
            var customer = SeedCustomer();

            var dto = new CreateQuoteDto
            {
                CustomerId = customer.Id,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentConditions = "2x",
                Items = new List<QuoteItemDto>
                 {
                     new QuoteItemDto
                     {
                         Description = "Limpeza Geral",
                         Quantity = 1,
                         UnitPrice = 500
                     }
                 }
            };

            var response = await _client.PostAsJsonAsync("/api/quote", dto);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var parsed = JsonConvert.DeserializeObject<ApiResponse<QuoteResponseDto>>(body);

            Assert.IsNotNull(parsed);
            Assert.IsTrue(parsed.Success);
            Assert.IsTrue(parsed.Data.Id > 0);

            Assert.AreEqual(1, parsed.Data.QuoteItems.Count);

            var item = parsed.Data.QuoteItems.First();


            Assert.AreEqual("Limpeza Geral", item.Description);
            Assert.AreEqual(1, item.Quantity);
            Assert.AreEqual(500, item.UnitPrice);
            Assert.AreEqual(500, item.TotalPrice);
        }

        [TestMethod]
        public async Task GetQuoteById_ShouldReturn200_WhenExists()
        {
            var quote = SeedQuote();

            var response = await _client.GetAsync($"/api/quote/{quote.Id}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = JsonConvert.DeserializeObject<ApiResponse<QuoteResponseDto>>(
                await response.Content.ReadAsStringAsync());

            Assert.IsNotNull(result);
            Assert.AreEqual(quote.TotalPrice, result.Data.TotalPrice);
        }


        [TestMethod]
        public async Task GetQuoteById_ShouldReturn404_WhenNotExists()
        {
            var response = await _client.GetAsync("/api/quote/99999");

            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }


        [TestMethod]
        public async Task UpdateQuote_ShouldReturn200_WhenValid()
        {
            var quote = SeedQuote();

            var dto = new UpdateQuoteDto
            {
                Id = quote.Id,
                TotalPrice = 999,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "À vista"
            };

            var response = await _client.PutAsJsonAsync($"/api/quote/{quote.Id}", dto);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = JsonConvert.DeserializeObject<ApiResponse<QuoteResponseDto>>(
                await response.Content.ReadAsStringAsync());

            Assert.IsNotNull(result);
            Assert.AreEqual(999, result.Data.TotalPrice);
        }


        [TestMethod]
        public async Task DeleteQuote_ShouldReturn204_WhenExists()
        {
            var quote = SeedQuote();

            var response = await _client.DeleteAsync($"/api/quote/{quote.Id}");

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
        }


        [TestMethod]
        public async Task DeleteQuote_ShouldReturn404_WhenNotExists()
        {
            var response = await _client.DeleteAsync("/api/quote/99999");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }


        [TestMethod]
        public async Task SearchQuotes_ShouldReturn10Items_AndRespectSortOrder()
        {
            for (int i = 0; i < 15; i++)
                SeedQuote();

            var filter = new QuoteFilterDto
            {
                Page = 1,
                PageSize = 10
            };

            var response = await _client.PostAsJsonAsync("/api/quotes/search", filter);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content
                .ReadFromJsonAsync<ApiResponse<PagedResult<QuoteResponseDto>>>();

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Data);

            Assert.AreEqual(10, result.Data.Items.Count);

            var expectedFirst = _db.Quotes
                .OrderByDescending(q => q.CreatedAt)
                .First();

            var firstReturned = result.Data.Items.First();

            Assert.AreEqual(expectedFirst.Id, firstReturned.Id);
            Assert.AreEqual(expectedFirst.TotalPrice, firstReturned.TotalPrice);
        }

        [TestMethod]
        public async Task CreateQuote_ShouldReturn400_WhenItemsEmpty()
        {
            var customer = SeedCustomer();

            var dto = new CreateQuoteDto
            {
                CustomerId = customer.Id,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "À vista",
                Items = new List<QuoteItemDto>() 
            };

            var response = await _client.PostAsJsonAsync("/api/quote", dto);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
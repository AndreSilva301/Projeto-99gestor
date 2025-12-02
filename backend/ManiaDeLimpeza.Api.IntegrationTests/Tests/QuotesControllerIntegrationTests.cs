using ManiaDeLimpeza.Api.IntegrationTests.Tools;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Infrastructure.Helpers;
using ManiaDeLimpeza.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests;

[TestClass]
public class QuoteControllerTests
{
    private HttpClient _client;
    private CustomWebApplicationFactory _factory;

    [TestInitialize]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory();
        _client = _factory.CreateClient();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    }

    [TestCleanup]
    public void Cleanup()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        TestDataCleanup.ClearQuotes(db);
        TestDataCleanup.ClearCustomers(db);
        TestDataCleanup.ClearUsers(db);
        TestDataCleanup.ClearCompany(db);
    }
    private async Task<string> LoginAsync(string email, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });

        var body = await response.Content.ReadAsStringAsync();
        var parsed = JsonConvert.DeserializeObject<ApiResponse<AuthResponseDto>>(body);

        Assert.IsTrue(parsed.Success, "Login failed during test setup.");

        return parsed.Data.BearerToken;
    }
    public static class TestDataFactory
    {
        public static Company CreateCompany(ApplicationDbContext db, string name = "Company Test")
        {
            var company = new Company { Name = name };
            db.Companies.Add(company);
            db.SaveChanges();
            return company;
        }

        public static User CreateUserAdmin(ApplicationDbContext db, Company company, string email = "admin@test.com", string password = "123456")
        {
            var user = new User
            {
                Email = email,
                Name = "Admin",
                CompanyId = company.Id,
                Profile = UserProfile.Admin
            };

            user.PasswordHash = PasswordHelper.Hash(password, user);

            db.Users.Add(user);
            db.SaveChanges();

            return user;
        }

        public static Customer CreateCustomer(ApplicationDbContext db, Company company, string name = "Cliente Teste")
        {
            var customer = new Customer
            {
                Name = name,
                CompanyId = company.Id,
                Phone = new Phone { Mobile = "11999999999" }
            };

            db.Customers.Add(customer);
            db.SaveChanges();

            return customer;
        }

        public static Quote CreateQuote(ApplicationDbContext db, Company company, Customer customer, User user, decimal total = 150)
        {
            var quote = new Quote
            {
                CustomerId = customer.Id,
                CompanyId = company.Id,
                UserId = user.Id,
                PaymentMethod = PaymentMethod.Pix,
                TotalPrice = total
            };

            db.Quotes.Add(quote);
            db.SaveChanges();

            return quote;
        }

        public static CreateQuoteDto CreateQuoteDto(int customerId)
        {
            return new CreateQuoteDto
            {
                CustomerId = customerId,
                PaymentMethod = PaymentMethod.CreditCard,
                Items = new List<QuoteItemDto>
            {
                new QuoteItemDto
                {
                    Description = "Serviço Teste",
                    Quantity = 2,
                    UnitPrice = 100
                }
            }
            };
        }

        public static UpdateQuoteDto CreateUpdateQuoteDto(int id, decimal total = 500)
        {
            return new UpdateQuoteDto
            {
                Id = id,
                PaymentMethod = PaymentMethod.Cash,
                TotalPrice = total,
                Items = new List<UpdateQuoteItemDto>
            {
                new UpdateQuoteItemDto
                {
                    Description = "Teste",
                    Quantity = 1,
                    UnitPrice = total
                }
            }
            };
        }
    }

    [TestMethod]
    public async Task CreateQuote_ShouldCreateSuccessfully()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = TestDataFactory.CreateCompany(db);
        var user = TestDataFactory.CreateUserAdmin(db, company);
        var customer = TestDataFactory.CreateCustomer(db, company);

        var token = await LoginAsync(user.Email, "123456");

        var quoteDto = TestDataFactory.CreateQuoteDto(customer.Id);

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/quote");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(quoteDto);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
    }

    [TestMethod]
    public async Task UpdateQuote_ShouldReturn403_WhenQuoteDoesNotBelongToCompany()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var companyA = TestDataFactory.CreateCompany(db, "Empresa A");
        var companyB = TestDataFactory.CreateCompany(db, "Empresa B");

        var userA = TestDataFactory.CreateUserAdmin(db, companyA, "userA@test.com", "Senha123");

        var token = await LoginAsync("userA@test.com", "Senha123");

        var customer = TestDataFactory.CreateCustomer(db, companyB);
        var quote = TestDataFactory.CreateQuote(db, companyB, customer, userA);

        var dto = TestDataFactory.CreateUpdateQuoteDto(quote.Id);

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/quote/{quote.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(dto);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

        var parsed = JsonConvert.DeserializeObject<ApiResponse<string>>(
            await response.Content.ReadAsStringAsync()
        );

        Assert.IsFalse(parsed.Success);
        Assert.AreEqual("Quote does not belong to the company.", parsed.Message);
    }

    [TestMethod]
    public async Task GetById_ShouldReturnQuote_WhenExistsAndBelongsToCompany()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = TestDataFactory.CreateCompany(db);
        var user = TestDataFactory.CreateUserAdmin(db, company);
        var customer = TestDataFactory.CreateCustomer(db, company);
        var quote = TestDataFactory.CreateQuote(db, company, customer, user);

        var token = await LoginAsync(user.Email, "123456");

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/quote/{quote.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var parsed = JsonConvert.DeserializeObject<ApiResponse<QuoteResponseDto>>(
            await response.Content.ReadAsStringAsync()
        );

        Assert.AreEqual(quote.Id, parsed.Data.Id);
    }

    [TestMethod]
    public async Task Delete_ShouldReturn204_WhenSuccessful()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = TestDataFactory.CreateCompany(db);
        var user = TestDataFactory.CreateUserAdmin(db, company);
        var customer = TestDataFactory.CreateCustomer(db, company);
        var quote = TestDataFactory.CreateQuote(db, company, customer, user);

        var token = await LoginAsync(user.Email, "123456");

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/quote/{quote.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [TestMethod]
    public async Task CreateQuote_ShouldReturnBadRequest_WhenInvalidItems()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = TestDataFactory.CreateCompany(db);
        var user = TestDataFactory.CreateUserAdmin(db, company);
        var customer = TestDataFactory.CreateCustomer(db, company);

        var token = await LoginAsync(user.Email, "123456");

        var invalidDto = new CreateQuoteDto
        {
            CustomerId = customer.Id,
            PaymentMethod = PaymentMethod.CreditCard,
            Items = new List<QuoteItemDto>
        {
            new QuoteItemDto
            {
                Description = "", 
                Quantity = 0,     
                UnitPrice = -10   
            }
        }
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/quote");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(invalidDto);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task UpdateQuote_ShouldReturnBadRequest_WhenInvalidPayload()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = TestDataFactory.CreateCompany(db);
        var user = TestDataFactory.CreateUserAdmin(db, company);

        var token = await LoginAsync(user.Email, "123456");

        var invalidDto = new UpdateQuoteDto
        {
            Id = 0, 
            PaymentMethod = PaymentMethod.Cash,
            TotalPrice = -200, 
            Items = new List<UpdateQuoteItemDto>()
        };

        var request = new HttpRequestMessage(HttpMethod.Put, "/api/quote/0");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(invalidDto);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task UpdateQuote_ShouldReturnNotFound_WhenQuoteDoesNotExist()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = TestDataFactory.CreateCompany(db);
        var user = TestDataFactory.CreateUserAdmin(db, company);

        var token = await LoginAsync(user.Email, "123456");

        var dto = TestDataFactory.CreateUpdateQuoteDto(999999);

        var request = new HttpRequestMessage(HttpMethod.Put, "/api/quote/999999");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(dto);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task GetById_ShouldReturnNotFound_WhenQuoteDoesNotExist()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = TestDataFactory.CreateCompany(db);
        var user = TestDataFactory.CreateUserAdmin(db, company);

        var token = await LoginAsync(user.Email, "123456");

        var request = new HttpRequestMessage(HttpMethod.Get, "/api/quote/999999");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task GetById_ShouldReturnForbidden_WhenQuoteBelongsToOtherCompany()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var companyA = TestDataFactory.CreateCompany(db);
        var companyB = TestDataFactory.CreateCompany(db);

        var userA = TestDataFactory.CreateUserAdmin(db, companyA);
        var customerB = TestDataFactory.CreateCustomer(db, companyB);

        var quoteB = TestDataFactory.CreateQuote(db, companyB, customerB, userA);

        var token = await LoginAsync(userA.Email, "123456");

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/quote/{quoteB.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
    }

    [TestMethod]
    public async Task Delete_ShouldReturnNotFound_WhenQuoteDoesNotExist()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = TestDataFactory.CreateCompany(db);
        var user = TestDataFactory.CreateUserAdmin(db, company);

        var token = await LoginAsync(user.Email, "123456");

        var request = new HttpRequestMessage(HttpMethod.Delete, "/api/quote/999999");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task Delete_ShouldReturnForbidden_WhenQuoteBelongsToOtherCompany()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var companyA = TestDataFactory.CreateCompany(db);
        var companyB = TestDataFactory.CreateCompany(db);

        var userA = TestDataFactory.CreateUserAdmin(db, companyA);
        var customerB = TestDataFactory.CreateCustomer(db, companyB);
        var quoteB = TestDataFactory.CreateQuote(db, companyB, customerB, userA);

        var token = await LoginAsync(userA.Email, "123456");

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/quote/{quoteB.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [TestMethod]
    public async Task SearchQuotes_ShouldReturnPagedResults_WhenMatchesFound()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = TestDataFactory.CreateCompany(db);
        var user = TestDataFactory.CreateUserAdmin(db, company);
        var customer = TestDataFactory.CreateCustomer(db, company);

        var token = await LoginAsync(user.Email, "123456");

        TestDataFactory.CreateQuote(db, company, customer, user);
        TestDataFactory.CreateQuote(db, company, customer, user);

        var filter = new QuoteFilterDto
        {
            Page = 1,
            PageSize = 10
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/quote/search");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(filter);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var parsed = JsonConvert.DeserializeObject<ApiResponse<PagedResult<QuoteResponseDto>>>(
            await response.Content.ReadAsStringAsync()
        );

        Assert.IsTrue(parsed.Data.TotalItems >= 2);
    }

    [TestMethod]
    public async Task SearchQuotes_ShouldReturnEmpty_WhenNoMatches()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = TestDataFactory.CreateCompany(db);
        var user = TestDataFactory.CreateUserAdmin(db, company);

        var token = await LoginAsync(user.Email, "123456");

        var filter = new QuoteFilterDto
        {
            Page = 1,
            PageSize = 10
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/quote/search");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(filter);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var parsed = JsonConvert.DeserializeObject<ApiResponse<PagedResult<QuoteResponseDto>>>(
            await response.Content.ReadAsStringAsync()
        );

        Assert.AreEqual(0, parsed.Data.TotalItems);
    }

    [TestMethod]
    public async Task SearchQuotes_ShouldRespectPagination()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = TestDataFactory.CreateCompany(db);
        var user = TestDataFactory.CreateUserAdmin(db, company);
        var customer = TestDataFactory.CreateCustomer(db, company);

        var token = await LoginAsync(user.Email, "123456");

        for (int i = 0; i < 15; i++)
            TestDataFactory.CreateQuote(db, company, customer, user);

        var filter = new QuoteFilterDto
        {
            Page = 2,
            PageSize = 5
        };

        var request = new HttpRequestMessage(HttpMethod.Post, "/api/quote/search");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(filter);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var parsed = JsonConvert.DeserializeObject<ApiResponse<PagedResult<QuoteResponseDto>>>(
            await response.Content.ReadAsStringAsync()
        );

        Assert.AreEqual(5, parsed.Data.Items.Count);
    }
}
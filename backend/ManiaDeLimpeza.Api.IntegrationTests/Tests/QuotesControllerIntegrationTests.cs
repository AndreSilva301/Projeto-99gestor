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

    [TestMethod]
    private async Task<string> LoginAsync(string email, string password)
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });

        var body = await loginResponse.Content.ReadAsStringAsync();
        var parsed = JsonConvert.DeserializeObject<ApiResponse<AuthResponseDto>>(body);

        Assert.IsTrue(parsed.Success, "Falha no login");

        return parsed.Data.BearerToken;
    }

    [TestMethod]
    public async Task CreateQuote_ShouldCreateSuccessfully()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = new Company { Name = "Company A" };
        db.Companies.Add(company);
        db.SaveChanges();

        var user = new User
        {
            Email = "user@test.com",
            Name = "User",
            CompanyId = company.Id,
            Profile = UserProfile.Admin
        };
        user.PasswordHash = PasswordHelper.Hash("123456", user);
        db.Users.Add(user);
        db.SaveChanges();

        var customer = new Customer
        {
            Name = "Cliente",
            CompanyId = company.Id,
            Phone = new Phone { Mobile = "11999999999" }
        };
        db.Customers.Add(customer);
        db.SaveChanges();

        var token = await LoginAsync(user.Email, "123456");

        var quoteDto = new CreateQuoteDto
        {
            CustomerId = customer.Id,
            PaymentMethod = PaymentMethod.CreditCard,
            Items = new List<QuoteItemDto>
            {
               new QuoteItemDto
               {
                   Description = "Limpeza Geral",
                   Quantity = 3,
                   UnitPrice = 150
               }
            }
        };

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

        var companyA = new Company { Name = "Empresa A" };
        var companyB = new Company { Name = "Empresa B" };
        db.Companies.AddRange(companyA, companyB);
        db.SaveChanges();

        var userA = new User
        {
            Email = "userA@test.com",
            Name = "User A",
            CompanyId = companyA.Id,
            Profile = UserProfile.Admin
        };
        userA.PasswordHash = PasswordHelper.Hash("Senha123", userA);
        db.Users.Add(userA);
        db.SaveChanges();

        var token = await LoginAsync("userA@test.com", "Senha123");

        var customer = new Customer
        {
            Name = "Cliente",
            CompanyId = companyB.Id,
            Phone = new Phone { Mobile = "11999999999" }
        };
        db.Customers.Add(customer);
        db.SaveChanges();

        var quote = new Quote
        {
            CustomerId = customer.Id,
            CompanyId = companyB.Id,
            UserId = userA.Id,
            PaymentMethod = PaymentMethod.Pix,
            TotalPrice = 200
        };
        db.Quotes.Add(quote);
        db.SaveChanges();

        var dto = new UpdateQuoteDto
        {
            Id = quote.Id,
            TotalPrice = 500,
            PaymentMethod = PaymentMethod.Cash,
            Items = new List<UpdateQuoteItemDto>
            {
                new UpdateQuoteItemDto
                {
                    Description = "Teste",
                    Quantity = 1,
                    UnitPrice = 500
                }
            }
        };

        var request = new HttpRequestMessage(HttpMethod.Put, $"/api/quote/{quote.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(dto);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var parsed = JsonConvert.DeserializeObject<ApiResponse<string>>(body);

        Assert.IsFalse(parsed.Success);
        Assert.AreEqual("Quote does not belong to the company.", parsed.Message);
    }

    [TestMethod]
    public async Task GetById_ShouldReturnQuote_WhenExistsAndBelongsToCompany()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = new Company { Name = "Empresa A" };
        db.Companies.Add(company);
        db.SaveChanges();

        var user = new User
        {
            Email = "user@test.com",
            Name = "User",
            CompanyId = company.Id,
            Profile = UserProfile.Admin
        };
        user.PasswordHash = PasswordHelper.Hash("123456", user);

        db.Users.Add(user);
        db.SaveChanges();

        var token = await LoginAsync("user@test.com", "123456");

        var customer = new Customer
        {
            Name = "Cliente",
            CompanyId = company.Id,
            Phone = new Phone { Mobile = "11999999999" }
        };

        db.Customers.Add(customer);
        db.SaveChanges();

        var quote = new Quote
        {
            CustomerId = customer.Id,
            CompanyId = company.Id,
            UserId = user.Id,
            PaymentMethod = PaymentMethod.Pix,
            TotalPrice = 300
        };

        db.Quotes.Add(quote);
        db.SaveChanges();

        var request = new HttpRequestMessage(HttpMethod.Get, $"/api/quote/{quote.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var body = await response.Content.ReadAsStringAsync();
        var parsed = JsonConvert.DeserializeObject<ApiResponse<QuoteResponseDto>>(body);

        Assert.AreEqual(quote.Id, parsed.Data.Id);
    }

    [TestMethod]
    public async Task Delete_ShouldReturn204_WhenSuccessful()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var company = new Company { Name = "Empresa A" };
        db.Companies.Add(company);
        db.SaveChanges();

        var user = new User
        {
            Email = "user@test.com",
            Name = "User",
            CompanyId = company.Id,
            Profile = UserProfile.Admin
        };

        user.PasswordHash = PasswordHelper.Hash("123456", user);
        db.Users.Add(user);
        db.SaveChanges();

        var token = await LoginAsync("user@test.com", "123456");

        var customer = new Customer
        {
            Name = "Cliente",
            CompanyId = company.Id,
            Phone = new Phone { Mobile = "11999999999" }
        };

        db.Customers.Add(customer);
        db.SaveChanges();

        var quote = new Quote
        {
            CustomerId = customer.Id,
            CompanyId = company.Id,
            UserId = user.Id,
            PaymentMethod = PaymentMethod.Pix,
            TotalPrice = 150
        };

        db.Quotes.Add(quote);
        db.SaveChanges();

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/quote/{quote.Id}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.SendAsync(request);

        Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);
    }

    [TestMethod]
    public void CreateQuote_ShouldReturnBadRequest_WhenInvalidItems()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void UpdateQuote_ShouldReturnBadRequest_WhenInvalidPayload()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void UpdateQuote_ShouldReturnNotFound_WhenQuoteDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void GetById_ShouldReturnNotFound_WhenQuoteDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void GetById_ShouldReturnForbidden_WhenQuoteBelongsToOtherCompany()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void Delete_ShouldReturnNotFound_WhenQuoteDoesNotExist()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void Delete_ShouldReturnForbidden_WhenQuoteBelongsToOtherCompany()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void SearchQuotes_ShouldReturnPagedResults_WhenMatchesFound()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void SearchQuotes_ShouldReturnEmpty_WhenNoMatches()
    {
        throw new NotImplementedException();
    }

    [TestMethod]
    public void SearchQuotes_ShouldRespectPagination()
    {
        throw new NotImplementedException();
    }
}
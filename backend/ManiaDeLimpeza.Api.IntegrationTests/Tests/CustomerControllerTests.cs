
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Infrastructure.Helpers;
using ManiaDeLimpeza.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests
{
    [TestClass]
    public class CustomerControllerTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private string _token;

        [TestInitialize]
        public async Task Setup()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.EnsureCreated();

                if (!db.Users.Any(u => u.Email == "admin@teste.com"))
                {
                    var user = new User
                    {
                        Email = "admin@teste.com",
                        Name = "Administrador",
                        CompanyId = 1
                    };

                    user.PasswordHash = PasswordHelper.Hash("123456", user);

                    db.Users.Add(user);
                    db.SaveChanges();
                }
            }

            var loginContent = new StringContent(JsonConvert.SerializeObject(new
            {
                Email = "admin@teste.com",
                Password = "123456"
            }), Encoding.UTF8, "application/json");

            var loginResponse = await _client.PostAsync("/api/auth/login", loginContent);
            var responseString = await loginResponse.Content.ReadAsStringAsync();

            if (!loginResponse.IsSuccessStatusCode)
                Assert.Fail($"Login falhou: {loginResponse.StatusCode} - {responseString}");

            dynamic loginResult = JsonConvert.DeserializeObject(responseString);

            _token = loginResult.data?.bearerToken?.ToString()
                  ?? loginResult.data?.BearerToken?.ToString();

            Assert.IsFalse(string.IsNullOrEmpty(_token), "Token JWT não retornado pelo login.");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);
        }

        [TestCleanup]
        public void Cleanup()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureDeleted();
        }

        [TestMethod]
        public async Task CreateCustomer_ShouldReturnCreated()
        {
            var dto = new
            {
                Name = "Cliente Teste",
                Phone = new
                {
                    Mobile = "11999999999",
                    Landline = "1133334444"
                },
                Email = "cliente@teste.com",
                Address = new
                {
                    Street = "Rua Teste",
                    Number = "123",
                    City = "São Paulo",
                    State = "SP",
                    ZipCode = "01000-000"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/customer", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Create Response: " + responseBody);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var created = db.Customers.FirstOrDefault(c => c.Email == dto.Email);
            Assert.IsNotNull(created);
            Assert.AreEqual(dto.Name, created.Name);
        }

        [TestMethod]
        public async Task GetById_WhenCustomerExists_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var customer = new Customer
            {
                CompanyId = 1,
                Name = "Cliente Existente",
                Email = "existe@teste.com"
            };
            db.Customers.Add(customer);
            db.SaveChanges();

            var response = await _client.GetAsync($"/api/customer/{customer.Id}");

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<CustomerDto>(json);
            Assert.IsNotNull(dto);
            Assert.AreEqual(customer.Name, dto.Name);
        }

        [TestMethod]
        public async Task GetById_WhenCustomerDoesNotExist_ShouldReturnNotFound()
        {
            var response = await _client.GetAsync("/api/customer/99999");
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateCustomer_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var existing = new Customer
            {
                CompanyId = 1,
                Name = "Antigo Nome",
                Email = "atualizar@teste.com"
            };
            db.Customers.Add(existing);
            db.SaveChanges();

            var updateDto = new CustomerUpdateDto
            {
                Name = "Novo Nome",
                Phone = new PhoneDto { Mobile = "11999998888", Landline = "1133334444" }
            };

            var content = new StringContent(JsonConvert.SerializeObject(updateDto), Encoding.UTF8, "application/json");
            var response = await _client.PutAsync($"/api/customer/{existing.Id}", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Update Response: " + responseBody);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updated = db.Customers.First(c => c.Id == existing.Id);
            Assert.AreEqual("Novo Nome", updated.Name);
        }

        [TestMethod]
        public async Task DeleteCustomer_ShouldReturnNoContent()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var customer = new Customer
            {
                CompanyId = 1,
                Name = "Deletável",
                Email = "delete@teste.com"
            };
            db.Customers.Add(customer);
            db.SaveChanges();

            var response = await _client.DeleteAsync($"/api/customer/{customer.Id}");
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Delete Response: " + responseBody);

            Assert.AreEqual(HttpStatusCode.NoContent, response.StatusCode);

            db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var deleted = db.Customers.FirstOrDefault(c => c.Id == customer.Id);
            Assert.IsTrue(deleted!.IsDeleted);
        }

        [TestMethod]
        public async Task SearchCustomer_ShouldReturnPagedResults()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            db.Customers.Add(new Customer { CompanyId = 1, Name = "Carlos Lima", Email = "carlos@teste.com" });
            db.Customers.Add(new Customer { CompanyId = 1, Name = "Carla Souza", Email = "carla@teste.com" });
            db.SaveChanges();

            var response = await _client.GetAsync("/api/customer/search?term=Car&page=1&pageSize=10");
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Search Response: " + responseBody);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var result = JsonConvert.DeserializeObject<PagedResult<CustomerListItemDto>>(responseBody);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Items.Any());
        }
    }
}

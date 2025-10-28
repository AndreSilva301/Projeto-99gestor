using ManiaDeLimpeza.Api.IntegrationTests.Tools;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Text;


namespace ManiaDeLimpeza.Api.IntegrationTests.Tests
{
    [TestClass]
    public class CustomerControllerTests
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public CustomerControllerTests()
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();
        }
       
        [TestMethod]
        public async Task CreateCustomer_ShouldReturnCreated()
        {
            // Arrange
            var dto = new CustomerCreateDto
            {
                Name = "Cliente Teste",
                Phone = new PhoneDto
                {
                    Mobile = "11999999999",
                    Landline = "1133334444"
                },  
                Email = "cliente@teste.com",
                Address = new AddressDto
                {
                    Street = "Rua Teste",
                    Number = "123",
                    City = "São Paulo",
                    State = "SP",
                    ZipCode = "01000-000"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/customer", content);

            // Assert
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

            var customer = new Domain.Entities.Customer
            {
                CompanyId = 1,
                Name = "Cliente Existente",
                Email = "existe@teste.com"
            };
            db.Customers.Add(customer);
            db.SaveChanges();

            // Act
            var response = await _client.GetAsync($"/api/customer/{customer.Id}");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var dto = JsonConvert.DeserializeObject<CustomerDto>(json);
            Assert.IsNotNull(dto);
            Assert.AreEqual(customer.Name, dto.Name);
        }

        [TestMethod]
        public async Task GetById_WhenCustomerDoesNotExist_ShouldReturnNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/customer/99999");

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateCustomer_ShouldReturnOk()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var existing = new Domain.Entities.Customer
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

            // Act
            var response = await _client.PutAsync($"/api/customer/{existing.Id}", content);

            // Assert
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

            var customer = new Domain.Entities.Customer
            {
                CompanyId = 1,
                Name = "Deletável",
                Email = "delete@teste.com"
            };
            db.Customers.Add(customer);
            db.SaveChanges();

            // Act
            var response = await _client.DeleteAsync($"/api/customer/{customer.Id}");

            // Assert
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

            db.Customers.Add(new Domain.Entities.Customer { CompanyId = 1, Name = "Carlos Lima", Email = "carlos@teste.com" });
            db.Customers.Add(new Domain.Entities.Customer { CompanyId = 1, Name = "Carla Souza", Email = "carla@teste.com" });
            db.SaveChanges();

            // Act
            var response = await _client.GetAsync("/api/customer/search?term=Car&page=1&pageSize=10");

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<PagedResult<CustomerListItemDto>>(json);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Items.Any());
        }
    }
}


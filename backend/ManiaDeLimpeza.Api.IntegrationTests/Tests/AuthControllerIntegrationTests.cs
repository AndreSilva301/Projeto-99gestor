using ManiaDeLimpeza.Api.IntegrationTests.Tools;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests
{
    [TestClass]
    public class AuthControllerIntegrationTests
    {
        private static CustomWebApplicationFactory _factory;
        private static HttpClient _client;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TestCleanup]
        public void Cleanup()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            TestDataCleanup.ClearUsers(db);
            TestDataCleanup.ClearCompany(db);
        }

        [TestMethod]
        public async Task Register_ShouldReturnCreated()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Name = "Test User",
                Email = "testuser@example.com",
                Password = "Secure123",
                ConfirmPassword = "Secure123",
                AcceptTerms = true,
                CompanyName = "Test Company",
                Phone = "1234567890"
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
        }


        [TestMethod]
        public async Task Register_WhenPasswordNotMatch_ShouldBreak()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Name = "Test User",
                Email = "testuser@example.com",
                Password = "Secure123",
                ConfirmPassword = "Secure1234",
                AcceptTerms = true,
                CompanyName = "Test Company",
                Phone = "1234567890"
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        }


        [TestMethod]
        public async Task Register_WhenEmailIsInvalid_ShouldBreak()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Name = "Test User",
                Email = "testuser",
                Password = "Secure123",
                ConfirmPassword = "Secure123",
                AcceptTerms = true,
                CompanyName = "Test Company",
                Phone = "1234567890"
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        }


        [TestMethod]
        public async Task Register_WhenEmailIsDuplicated_ShouldBreak()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Name = "Test User",
                Email = "testuser@test.com",
                Password = "Secure123",
                ConfirmPassword = "Secure123",
                AcceptTerms = true,
                CompanyName = "Test Company",
                Phone = "1234567890"
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/register", content);

            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var registedUser = db.Users.SingleOrDefault(u => u.Email == registerDto.Email);
            Assert.IsNotNull(registedUser);
            Assert.AreEqual(registerDto.Name, registedUser.Name);
            Assert.AreEqual(registerDto.Email, registedUser.Email);

            registerDto = new RegisterUserDto
            {
                Name = "Test User2",
                Email = "testuser@test.com",
                Password = "Secure123",
                ConfirmPassword = "Secure123",
                AcceptTerms = true,
                CompanyName = "Test Company",
                Phone = "123456780"
            };

            content = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");

            response = await _client.PostAsync("/api/auth/register", content);

            // Act
            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [TestMethod]
        public async Task Register_WhenUsuarioNaoEhCriado_NaoDuplicarEmpresa()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Name = "Test User",
                Phone = "1234567890",
                CompanyName = "Test Company",
                Email = "testuser@test.com",
                Password = "Secure123",
                ConfirmPassword = "Secure123",
                AcceptTerms = true
               
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/register", content);

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            registerDto = new RegisterUserDto
            {
                Name = "Test User2",
                Phone = "1234567890",
                CompanyName = "Test Company",
                Email = "testuser@test.com",
                Password = "Secure123",
                ConfirmPassword = "Secure123",
                AcceptTerms = true
            };

            content = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");

            response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var empresasRegistradas = db.Companies
                .Where(c => c.Name == registerDto.CompanyName)
                .ToList();

            Assert.AreEqual(1, empresasRegistradas.Count);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }


        [TestMethod]
        public async Task Register_WhenPhoneIsInvalid_ShouldBreak()
        {
            // Arrange
            var registerDto = new RegisterUserDto
            {
                Name = "Test User",
                Email = "testuser@test.com",
                Password = "Secure123",
                ConfirmPassword = "Secure123",
                AcceptTerms = true,
                CompanyName = "Test Company",
                Phone = "1234567890123123123123123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

        }

        [TestMethod]
        public async Task Login_ShouldReturnOk()
        {
            SeedUserDatabase();

            var seedUser = TestDataSeeder.GetDefaultUser();

            // Arrange
            var loginDto = new
            {
                Email = seedUser.Email,
                Password = TestDataSeeder.DefaultPassword,
            };
            var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(!string.IsNullOrWhiteSpace(body));
        }


        [TestMethod]
        public async Task Login_ShouldReturnUnauthorized()
        {
            SeedUserDatabase();

            var seedUser = TestDataSeeder.GetDefaultUser();

            // Arrange
            var loginDto = new
            {
                Email = seedUser.Email,
                Password = "WrongPassword",
            };
            var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        private void SeedUserDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            TestDataSeeder.SeedDefaultUser(db);
        }
    }
}

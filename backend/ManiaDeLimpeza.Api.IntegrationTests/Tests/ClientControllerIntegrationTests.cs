using ManiaDeLimpeza.Api.IntegrationTests.Tools;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests
{
    [TestClass]
    public class ClientControllerIntegrationTests
    {
        private static CustomWebApplicationFactory _factory;
        private static HttpClient _client;

        [ClassInitialize]
        public static async Task ClassInit(TestContext context)
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
            await AuthenticateAsync();
        }


        [TestCleanup]
        public void Cleanup()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            TestDataCleanup.ClearUsers(db);
            TestDataCleanup.ClearClients(db);
        }

        private static async Task AuthenticateAsync()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var user = new User
            {
                Name = "Test User",
                Email = "testuser@example.com",
                IsActive = true,
                Password = Infrastructure.Helpers.PasswordHelper.Hash("Secure123", new User { Email = "testuser@example.com" })
            };

            db.Users.Add(user);
            db.SaveChanges();

            var login = new { Email = "testuser@example.com", Password = "Secure123" };
            var response = await _client.PostAsync("/api/auth/login",
                new StringContent(JsonConvert.SerializeObject(login), Encoding.UTF8, "application/json"));

            var json = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<AuthResponseDto>(json);
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", obj!.BearerToken);
        }


        [TestMethod]
        public async Task CreateAndGetClient_ShouldSucceed()
        {
            var client = new Client
            {
                Name = "Ana Silva",
                Email = "ana@example.com",
                Phone = new Phone { Mobile = "9999-0000" },
                Address = new Address { Street = "Rua A", Number = "123", City = "City" }
            };

            var response = await _client.PostAsync("/api/client",
                new StringContent(JsonConvert.SerializeObject(client), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var created = JsonConvert.DeserializeObject<Client>(json);

            var getResponse = await _client.GetAsync($"/api/client/{created!.Id}");
            Assert.AreEqual(HttpStatusCode.OK, getResponse.StatusCode);

            var fetched = JsonConvert.DeserializeObject<Client>(await getResponse.Content.ReadAsStringAsync());
            Assert.AreEqual("Ana Silva", fetched!.Name);
        }


        [TestMethod]
        public async Task UpdateClient_ShouldSucceed()
        {
            var client = new Client
            {
                Name = "Ana",
                Email = "ana@update.com",
                Phone = new Phone { Mobile = "0000-1111" },
                Address = new Address { Street = "Rua B", Number = "11", City = "City" }
            };

            var post = await _client.PostAsync("/api/client",
                new StringContent(JsonConvert.SerializeObject(client), Encoding.UTF8, "application/json"));
            var created = JsonConvert.DeserializeObject<Client>(await post.Content.ReadAsStringAsync());

            created!.Name = "Ana Updated";
            var put = await _client.PutAsync($"/api/client/{created.Id}",
                new StringContent(JsonConvert.SerializeObject(created), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.NoContent, put.StatusCode);

            var check = await _client.GetAsync($"/api/client/{created.Id}");
            var fetched = JsonConvert.DeserializeObject<Client>(await check.Content.ReadAsStringAsync());
            Assert.AreEqual("Ana Updated", fetched!.Name);
        }

        [TestMethod]
        public async Task DeleteClient_ShouldSucceed()
        {
            var client = new Client { Name = "Delete Me", Email = "delete@example.com" };
            var post = await _client.PostAsync("/api/client",
                new StringContent(JsonConvert.SerializeObject(client), Encoding.UTF8, "application/json"));
            var created = JsonConvert.DeserializeObject<Client>(await post.Content.ReadAsStringAsync());

            var delete = await _client.DeleteAsync($"/api/client/{created!.Id}");
            Assert.AreEqual(HttpStatusCode.NoContent, delete.StatusCode);

            var check = await _client.GetAsync($"/api/client/{created.Id}");
            Assert.AreEqual(HttpStatusCode.NotFound, check.StatusCode);
        }

        [TestMethod]
        public async Task SearchClient_ShouldReturnMatch()
        {
            var client = new Client
            {
                Name = "Carlos Souza",
                Email = "carlos@example.com",
                Phone = new Phone { Landline = "2222-3333" }
            };

            await _client.PostAsync("/api/client",
                new StringContent(JsonConvert.SerializeObject(client), Encoding.UTF8, "application/json"));

            var search = await _client.GetAsync("/api/client/search?term=carlos 3333");
            var result = JsonConvert.DeserializeObject<PagedResult<Client>>(await search.Content.ReadAsStringAsync());

            Assert.AreEqual(1, result.Items!.Count);
            Assert.AreEqual("Carlos Souza", result.Items[0].Name);
        }

    }
}

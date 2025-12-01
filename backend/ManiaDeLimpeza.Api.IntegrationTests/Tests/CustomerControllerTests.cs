using ManiaDeLimpeza.Api.IntegrationTests.Tools;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Infrastructure.Helpers;
using ManiaDeLimpeza.Persistence;
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
        private static CustomWebApplicationFactory _factory = null!;
        private static HttpClient _client = null!;
        private static Company _testCompany = null!;
        private static string _systemAdminToken = null!;
        private static string _adminToken = null!;
        private static string _employeeToken = null!;
        private static int _systemAdminUserId;
        private static int _adminUserId;
        private static int _employeeUserId;

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _factory = new CustomWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [TestInitialize]
        public async Task Setup()
        {
            // Cleanup before each test
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            TestDataCleanup.ClearCustomers(db);
            TestDataCleanup.ClearUsers(db);
            TestDataCleanup.ClearCompany(db);

            // Create test company
            _testCompany = new Company
            {
                Name = "Empresa Teste Customer",
                CNPJ = "71142891000169",
                Address = new Address
                {
                    Street = "Rua Teste",
                    Number = "123",
                    Neighborhood = "Centro",
                    City = "São Paulo",
                    State = "SP",
                    ZipCode = "01000-000"
                },
                Phone = new Phone
                {
                    Mobile = "11999999999"
                }
            };

            db.Companies.Add(_testCompany);
            await db.SaveChangesAsync();

            // Create SystemAdmin user
            var systemAdmin = new User
            {
                Name = "System Admin",
                Email = "sysadmin@teste.com",
                CompanyId = _testCompany.Id,
                Profile = UserProfile.SystemAdmin
            };
            systemAdmin.PasswordHash = PasswordHelper.Hash("Senha123", systemAdmin);
            db.Users.Add(systemAdmin);

            // Create Admin user
            var admin = new User
            {
                Name = "Admin User",
                Email = "admin@teste.com",
                CompanyId = _testCompany.Id,
                Profile = UserProfile.Admin
            };
            admin.PasswordHash = PasswordHelper.Hash("Senha123", admin);
            db.Users.Add(admin);

            // Create Employee user
            var employee = new User
            {
                Name = "Funcionário",
                Email = "employee@teste.com",
                CompanyId = _testCompany.Id,
                Profile = UserProfile.Employee
            };
            employee.PasswordHash = PasswordHelper.Hash("Senha123", employee);
            db.Users.Add(employee);

            await db.SaveChangesAsync();

            _systemAdminUserId = systemAdmin.Id;
            _adminUserId = admin.Id;
            _employeeUserId = employee.Id;

            // Get tokens for all users
            _systemAdminToken = await GetAuthToken("sysadmin@teste.com", "Senha123");
            _adminToken = await GetAuthToken("admin@teste.com", "Senha123");
            _employeeToken = await GetAuthToken("employee@teste.com", "Senha123");
        }

        [TestCleanup]
        public void Cleanup()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            TestDataCleanup.ClearCustomers(db);
            TestDataCleanup.ClearUsers(db);
            TestDataCleanup.ClearCompany(db);
        }

        private async Task<string> GetAuthToken(string email, string password)
        {
            var loginDto = new
            {
                Email = email,
                Password = password
            };

            var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/login", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<AuthResponseDto>>(responseBody);

            return apiResponse?.Data?.BearerToken ?? throw new Exception("Failed to get auth token");
        }

        #region Create Customer Tests

        [TestMethod]
        public async Task CreateCustomer_ShouldReturnCreated_WhenValidData()
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
                    ZipCode = "01000-000",
                    Neighborhood = "Centro"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/customer")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<CustomerDto>>(responseBody);

            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual(dto.Name, apiResponse.Data.Name);
            Assert.AreEqual(dto.Email, apiResponse.Data.Email);

            // Verify in database
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var created = db.Customers.FirstOrDefault(c => c.Email == dto.Email);

            Assert.IsNotNull(created);
            Assert.AreEqual(dto.Name, created.Name);
            Assert.AreEqual(_testCompany.Id, created.CompanyId);
        }

        [TestMethod]
        public async Task CreateCustomer_ShouldReturnBadRequest_WhenNameIsEmpty()
        {
            // Arrange
            var dto = new CustomerCreateDto
            {
                Name = "", // Invalid
                Email = "invalid@teste.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/customer")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody);

            Assert.IsNotNull(apiResponse);
            Assert.IsFalse(apiResponse.Success);
        }

        [TestMethod]
        public async Task CreateCustomer_ShouldReturnUnauthorized_WhenNoToken()
        {
            // Arrange
            var dto = new CustomerCreateDto
            {
                Name = "Cliente Teste",
                Email = "cliente@teste.com"
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/customer", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #region Get Customer Tests

        [TestMethod]
        public async Task GetById_ShouldReturnOk_WhenCustomerExists()
        {
            // Arrange
            int customerId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var customer = new Customer
                {
                    CompanyId = _testCompany.Id,
                    Name = "Cliente Existente",
                    Email = "existe@teste.com",
                    Address = new Address
                    {
                        Street = "Rua Teste",
                        Number = "123",
                        City = "São Paulo",
                        State = "SP",
                        ZipCode = "01000-000",
                        Neighborhood = "Centro"
                    },
                    Phone = new Phone
                    {
                        Mobile = "11999999999"
                    }
                };
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                customerId = customer.Id;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/customer/{customerId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<CustomerDto>>(json);

            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual("Cliente Existente", apiResponse.Data.Name);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/customer/99999");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnNotFound_WhenCustomerFromDifferentCompany()
        {
            // Arrange - Create another company with a customer
            int otherCustomerId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var otherCompany = new Company
                {
                    Name = "Outra Empresa",
                    CNPJ = "34028316000103"
                };
                db.Companies.Add(otherCompany);
                await db.SaveChangesAsync();

                var otherCustomer = new Customer
                {
                    CompanyId = otherCompany.Id,
                    Name = "Cliente Outra Empresa",
                    Email = "outro@teste.com"
                };
                db.Customers.Add(otherCustomer);
                await db.SaveChangesAsync();
                otherCustomerId = otherCustomer.Id;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/customer/{otherCustomerId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region Update Customer Tests

        [TestMethod]
        public async Task UpdateCustomer_ShouldReturnOk_WhenValidData()
        {
            // Arrange
            int customerId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var existing = new Customer
                {
                    CompanyId = _testCompany.Id,
                    Name = "Antigo Nome",
                    Email = "atualizar@teste.com",
                    Address = new Address
                    {
                        Street = "Rua Velha",
                        Number = "100",
                        City = "São Paulo",
                        State = "SP",
                        ZipCode = "01000-000",
                        Neighborhood = "Centro"
                    }
                };
                db.Customers.Add(existing);
                await db.SaveChangesAsync();
                customerId = existing.Id;
            }

            var updateDto = new CustomerUpdateDto
            {
                Name = "Novo Nome",
                Phone = new PhoneDto { Mobile = "11999998888", Landline = "1133334444" },
                Address = new AddressDto
                {
                    Street = "Rua Nova",
                    Number = "200",
                    City = "São Paulo",
                    State = "SP",
                    ZipCode = "02000-000",
                    Neighborhood = "Vila Nova"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(updateDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/customer/{customerId}")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<CustomerDto>>(responseBody);

            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual("Novo Nome", apiResponse.Data.Name);

            // Verify in database
            using var verifyScope = _factory.Services.CreateScope();
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updated = verifyDb.Customers.First(c => c.Id == customerId);

            Assert.AreEqual("Novo Nome", updated.Name);
            Assert.AreEqual("Rua Nova", updated.Address.Street);
        }

        [TestMethod]
        public async Task UpdateCustomer_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var updateDto = new CustomerUpdateDto
            {
                Name = "Nome Atualizado"
            };

            var content = new StringContent(JsonConvert.SerializeObject(updateDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, "/api/customer/99999")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task UpdateCustomer_ShouldReturnBadRequest_WhenInvalidData()
        {
            // Arrange
            int customerId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var customer = new Customer
                {
                    CompanyId = _testCompany.Id,
                    Name = "Cliente",
                    Email = "cliente@teste.com"
                };
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                customerId = customer.Id;
            }

            var updateDto = new CustomerUpdateDto
            {
                Name = "" // Invalid
            };

            var content = new StringContent(JsonConvert.SerializeObject(updateDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/customer/{customerId}")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Delete Customer Tests

        [TestMethod]
        public async Task DeleteCustomer_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            int customerId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var customer = new Customer
                {
                    CompanyId = _testCompany.Id,
                    Name = "Deletável",
                    Email = "delete@teste.com"
                };
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                customerId = customer.Id;
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/customer/{customerId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Verify soft delete in database
            using var verifyScope = _factory.Services.CreateScope();
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var deleted = verifyDb.Customers.FirstOrDefault(c => c.Id == customerId);

            Assert.IsNotNull(deleted);
            Assert.IsTrue(deleted.IsDeleted, "Customer should be soft deleted");
        }

        [TestMethod]
        public async Task DeleteCustomer_ShouldReturnNotFound_WhenCustomerDoesNotExist()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/customer/99999");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region Search Customer Tests

        [TestMethod]
        public async Task SearchCustomer_ShouldReturnPagedResults_WhenMatchesFound()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Customers.Add(new Customer { CompanyId = _testCompany.Id, Name = "Carlos Lima", Email = "carlos@teste.com" });
                db.Customers.Add(new Customer { CompanyId = _testCompany.Id, Name = "Carla Souza", Email = "carla@teste.com" });
                db.Customers.Add(new Customer { CompanyId = _testCompany.Id, Name = "Maria Silva", Email = "maria@teste.com" });
                await db.SaveChangesAsync();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/customer/search?term=Car&page=1&pageSize=10");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PagedResult<CustomerListItemDto>>>(responseBody);

            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.IsTrue(apiResponse.Data.Items.Count() >= 2, "Should find at least 2 customers with 'Car' in name");
            Assert.IsTrue(apiResponse.Data.Items.All(c => c.Name.Contains("Car")), "All results should contain 'Car'");
        }

        [TestMethod]
        public async Task SearchCustomer_ShouldReturnEmptyResults_WhenNoMatches()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/customer/search?term=NonExistent&page=1&pageSize=10");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PagedResult<CustomerListItemDto>>>(responseBody);

            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual(0, apiResponse.Data.Items.Count(), "Should return empty results");
        }

        [TestMethod]
        public async Task SearchCustomer_ShouldRespectPagination()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                for (int i = 1; i <= 15; i++)
                {
                    db.Customers.Add(new Customer
                    {
                        CompanyId = _testCompany.Id,
                        Name = $"Cliente {i}",
                        Email = $"cliente{i}@teste.com"
                    });
                }
                await db.SaveChangesAsync();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/customer/search?term=Cliente&page=1&pageSize=5");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PagedResult<CustomerListItemDto>>>(responseBody);

            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual(5, apiResponse.Data.Items.Count(), "Should return exactly 5 items per page");
            Assert.AreEqual(15, apiResponse.Data.TotalItems, "Should have 15 total items");
            Assert.AreEqual(3, apiResponse.Data.TotalPages, "Should have 3 pages");
        }

        [TestMethod]
        public async Task SearchCustomer_ShouldNotReturnDeletedCustomers()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Customers.Add(new Customer
                {
                    CompanyId = _testCompany.Id,
                    Name = "Cliente Ativo",
                    Email = "ativo@teste.com",
                    IsDeleted = false
                });
                db.Customers.Add(new Customer
                {
                    CompanyId = _testCompany.Id,
                    Name = "Cliente Deletado",
                    Email = "deletado@teste.com",
                    IsDeleted = true
                });
                await db.SaveChangesAsync();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/customer/search?term=Cliente&page=1&pageSize=10");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PagedResult<CustomerListItemDto>>>(responseBody);

            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.IsFalse(apiResponse.Data.Items.Any(c => c.Name == "Cliente Deletado"),
                "Should not return deleted customers");
            Assert.IsTrue(apiResponse.Data.Items.Any(c => c.Name == "Cliente Ativo"),
                "Should return active customers");
        }

        [TestMethod]
        public async Task SearchCustomer_ShouldOnlyReturnCustomersFromUserCompany()
        {
            // Arrange
            int otherCompanyId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var otherCompany = new Company
                {
                    Name = "Outra Empresa",
                    CNPJ = "34028316000103"
                };
                db.Companies.Add(otherCompany);
                await db.SaveChangesAsync();
                otherCompanyId = otherCompany.Id;

                db.Customers.Add(new Customer
                {
                    CompanyId = _testCompany.Id,
                    Name = "Cliente Minha Empresa",
                    Email = "meu@teste.com"
                });
                db.Customers.Add(new Customer
                {
                    CompanyId = otherCompanyId,
                    Name = "Cliente Outra Empresa",
                    Email = "outro@teste.com"
                });
                await db.SaveChangesAsync();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/customer/search?term=Cliente&page=1&pageSize=10");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<PagedResult<CustomerListItemDto>>>(responseBody);

            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.IsTrue(apiResponse.Data.Items.Any(c => c.Name == "Cliente Minha Empresa"),
                "Should return customers from user's company");
            Assert.IsFalse(apiResponse.Data.Items.Any(c => c.Name == "Cliente Outra Empresa"),
                "Should NOT return customers from other companies");
        }

        #endregion

        #region Customer Relationships Tests

        [TestMethod]
        public async Task AddRelationships_ShouldReturnCreated_WhenValidData()
        {
            // Arrange
            int customerId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var customer = new Customer
                {
                    CompanyId = _testCompany.Id,
                    Name = "Cliente com Relacionamento",
                    Email = "relacionamento@teste.com"
                };
                db.Customers.Add(customer);
                await db.SaveChangesAsync();
                customerId = customer.Id;
            }

            var relationships = new List<CustomerRelationshipDto>
            {
                new CustomerRelationshipDto
                {
                    Description = "Primeira visita",
                    DateTime = DateTime.Now.AddDays(-1)
                },
                new CustomerRelationshipDto
                {
                    Description = "Follow-up",
                    DateTime = DateTime.Now
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(relationships), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/customer/{customerId}/relationships")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            // Verify in database
            using var verifyScope = _factory.Services.CreateScope();
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var savedRelationships = verifyDb.CustomerRelationships
                .Where(r => r.CustomerId == customerId)
                .ToList();

            Assert.AreEqual(2, savedRelationships.Count);
            Assert.IsTrue(savedRelationships.Any(r => r.Description == "Primeira visita"));
            Assert.IsTrue(savedRelationships.Any(r => r.Description == "Follow-up"));
        }

        [TestMethod]
        public async Task GetRelationships_ShouldReturnOk_WhenRelationshipsExist()
        {
            // Arrange
            int customerId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var customer = new Customer
                {
                    CompanyId = _testCompany.Id,
                    Name = "Cliente",
                    Email = "cliente@teste.com"
                };
                db.Customers.Add(customer);
                await db.SaveChangesAsync();

                db.CustomerRelationships.Add(new CustomerRelationship
                {
                    CustomerId = customer.Id,
                    Description = "Relacionamento teste",
                    DateTime = DateTime.Now
                });
                await db.SaveChangesAsync();
                customerId = customer.Id;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/customer/{customerId}/relationships");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<CustomerRelationshipDto>>>(responseBody);

            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.IsTrue(apiResponse.Data.Any());
            Assert.IsTrue(apiResponse.Data.Any(r => r.Description == "Relacionamento teste"));
        }

        [TestMethod]
        public async Task DeleteRelationships_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            int customerId;
            int relationshipId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var customer = new Customer
                {
                    CompanyId = _testCompany.Id,
                    Name = "Cliente",
                    Email = "cliente@teste.com"
                };
                db.Customers.Add(customer);
                await db.SaveChangesAsync();

                var relationship = new CustomerRelationship
                {
                    CustomerId = customer.Id,
                    Description = "Para deletar",
                    DateTime = DateTime.Now
                };
                db.CustomerRelationships.Add(relationship);
                await db.SaveChangesAsync();

                customerId = customer.Id;
                relationshipId = relationship.Id;
            }

            var relationshipIds = new List<int> { relationshipId };
            var content = new StringContent(JsonConvert.SerializeObject(relationshipIds), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/customer/{customerId}/relationships")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            // Verify deletion in database
            using var verifyScope = _factory.Services.CreateScope();
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var deleted = verifyDb.CustomerRelationships.FirstOrDefault(r => r.Id == relationshipId);

            Assert.IsNull(deleted, "Relationship should be deleted");
        }

        #endregion
    }
}

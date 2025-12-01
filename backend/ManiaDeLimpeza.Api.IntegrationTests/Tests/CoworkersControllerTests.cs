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
    public class CoworkersControllerTests
    {
        private static CustomWebApplicationFactory _factory = null!;
        private static HttpClient _client = null!;
        private static Company _testCompany = null!;
        private static string _systemAdminToken = null!;
        private static string _adminToken = null!;
        private static string _employeeToken = null!;

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
            TestDataCleanup.ClearQuotes(db);
            TestDataCleanup.ClearCustomers(db);
            TestDataCleanup.ClearUsers(db);
            TestDataCleanup.ClearCompany(db);
           
            // Create test company
            _testCompany = new Company
            {
                Name = "Empresa Teste Coworkers",
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

        [TestMethod]
        public async Task GetAll_ShouldReturnForbid_WhenUserIsNotAdmin()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/coworkers");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);

            // Validate ApiResponse structure
            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody);

            Assert.IsNotNull(apiResponse, "API response should not be null");
            Assert.IsFalse(apiResponse.Success, "Success should be false for forbidden access");
            Assert.IsNotNull(apiResponse.Message, "Message should not be null");
            StringAssert.Contains(apiResponse.Message, "não autorizado", "Message should mention unauthorized access");
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk_WhenAdminRequests()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/coworkers");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var listOfUsers = JsonConvert.DeserializeObject<List<UserLightDto>>(responseBody);

            Assert.IsNotNull(listOfUsers, "Response should not be null");
            
            // Should return at least the employee (admin might be filtered out depending on business logic)
            Assert.IsTrue(listOfUsers.Count >= 1, "Should return at least one user");
            Assert.IsTrue(listOfUsers.Any(u => u.Email == "employee@teste.com"), "Should include the employee");
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk_WhenSystemAdminRequests()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, "/api/coworkers");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _systemAdminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var listOfUsers = JsonConvert.DeserializeObject<List<UserLightDto>>(responseBody);

            Assert.IsNotNull(listOfUsers, "Response should not be null");
            Assert.IsTrue(listOfUsers.Count >= 1, "Should return users from the company");
        }

        [TestMethod]
        public async Task CreateEmployee_ShouldReturnForbid_WhenNotAdmin()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                Name = "Novo Colaborador",
                Email = "novo@teste.com",
                ProfileType = UserProfile.Employee
            };

            var content = new StringContent(JsonConvert.SerializeObject(createDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/coworkers")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);

            // Validate ApiResponse structure
            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody);

            Assert.IsNotNull(apiResponse, "API response should not be null");
            Assert.IsFalse(apiResponse.Success, "Success should be false for forbidden access");
            StringAssert.Contains(apiResponse.Message, "administradores", "Message should mention administrators");
        }

        [TestMethod]
        public async Task CreateEmployee_ShouldReturnForbid_WhenProfileIsNotEmployee()
        {
            // Arrange
            var invalidDto = new CreateEmployeeDto
            {
                Name = "Sistema Admin",
                Email = "newsysadmin@teste.com",
                ProfileType = UserProfile.SystemAdmin
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/coworkers")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _systemAdminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody);

            Assert.IsNotNull(apiResponse, "API response should not be null");
            Assert.IsFalse(apiResponse.Success, "Success should be false");
            StringAssert.Contains(apiResponse.Message, "funcionários", "Message should mention employees only");
        }

        [TestMethod]
        public async Task CreateEmployee_ShouldReturnOk_WhenCreatedSuccessfully()
        {
            // Arrange
            var createDto = new CreateEmployeeDto
            {
                Name = "Novo Colaborador",
                Email = "novocolaborador@teste.com",
                ProfileType = UserProfile.Employee
            };

            var content = new StringContent(JsonConvert.SerializeObject(createDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/coworkers")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var userResponse = JsonConvert.DeserializeObject<UserLightDto>(responseBody);

            Assert.IsNotNull(userResponse, "Response should not be null");
            Assert.AreEqual("Novo Colaborador", userResponse.Name);
            Assert.AreEqual("novocolaborador@teste.com", userResponse.Email);
            Assert.AreEqual(UserProfile.Employee, userResponse.Profile);

            // Verify in database
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var createdUser = db.Users.FirstOrDefault(u => u.Email == "novocolaborador@teste.com");

            Assert.IsNotNull(createdUser, "User should be created in database");
            Assert.AreEqual(_testCompany.Id, createdUser.CompanyId, "User should belong to test company");
        }

        [TestMethod]
        public async Task Update_ShouldReturnOk_WhenUserUpdatesItself()
        {
            // Arrange
            int employeeId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                employeeId = db.Users.First(u => u.Email == "employee@teste.com").Id;
            }

            var updateDto = new UpdateUserDto
            {
                Name = "Colaborador Atualizado"
            };

            var content = new StringContent(JsonConvert.SerializeObject(updateDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/coworkers/{employeeId}")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var userResponse = JsonConvert.DeserializeObject<UserLightDto>(responseBody);

            Assert.IsNotNull(userResponse, "Response should not be null");
            Assert.AreEqual(updateDto.Name, userResponse.Name);
            Assert.AreEqual(UserProfile.Employee, userResponse.Profile);

            // Verify in database
            using var verifyScope = _factory.Services.CreateScope();
            var verifyDb = verifyScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updatedUser = await verifyDb.Users.FindAsync(employeeId);

            Assert.IsNotNull(updatedUser);
            Assert.AreEqual("Colaborador Atualizado", updatedUser.Name);
        }

        [TestMethod]
        public async Task Update_ShouldReturnForbid_WhenUserTriesToUpdateOtherUser()
        {
            // Arrange - Get admin's ID
            int adminId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                adminId = db.Users.First(u => u.Email == "admin@teste.com").Id;
            }

            var updateDto = new UpdateUserDto
            {
                Name = "Tentativa Atualização"
            };

            var content = new StringContent(JsonConvert.SerializeObject(updateDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/coworkers/{adminId}")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody);

            Assert.IsNotNull(apiResponse);
            Assert.IsFalse(apiResponse.Success);
            StringAssert.Contains(apiResponse.Message, "permissão", "Message should mention permission");
        }

        [TestMethod]
        public async Task Deactivate_ShouldReturnOk_WhenAdminDeactivatesUser()
        {
            // Arrange - Create a new employee to deactivate
            int employeeToDeactivateId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var newEmployee = new User
                {
                    Name = "Colaborador Para Desativar",
                    Email = "desativar@teste.com",
                    CompanyId = _testCompany.Id,
                    Profile = UserProfile.Employee
                };
                newEmployee.PasswordHash = PasswordHelper.Hash("Senha123", newEmployee);
                db.Users.Add(newEmployee);
                await db.SaveChangesAsync();
                employeeToDeactivateId = newEmployee.Id;
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/coworkers/{employeeToDeactivateId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var userResponse = JsonConvert.DeserializeObject<UserLightDto>(responseBody);

            Assert.IsNotNull(userResponse, "Response should not be null");
            Assert.AreEqual("Colaborador Para Desativar", userResponse.Name);
            Assert.AreEqual("desativar@teste.com", userResponse.Email);
            Assert.AreEqual(UserProfile.Inactive, userResponse.Profile, "User should be inactive");

            // Verify in database
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var deactivatedUser = await db.Users.FindAsync(employeeToDeactivateId);

                Assert.IsNotNull(deactivatedUser);
                Assert.AreEqual(UserProfile.Inactive, deactivatedUser.Profile, "User should be inactive in database");
            }
        }

        [TestMethod]
        public async Task Reactivate_ShouldReturnOk_WhenAdminReactivatesUser()
        {
            // Arrange - Create an inactive employee to reactivate
            int inactiveEmployeeId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var inactiveEmployee = new User
                {
                    Name = "Colaborador Inativo",
                    Email = "inativo@teste.com",
                    CompanyId = _testCompany.Id,
                    Profile = UserProfile.Inactive
                };
                inactiveEmployee.PasswordHash = PasswordHelper.Hash("Senha123", inactiveEmployee);
                db.Users.Add(inactiveEmployee);
                await db.SaveChangesAsync();
                inactiveEmployeeId = inactiveEmployee.Id;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/coworkers/{inactiveEmployeeId}/reactivate");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var userResponse = JsonConvert.DeserializeObject<UserLightDto>(responseBody);

            Assert.IsNotNull(userResponse, "Response should not be null");
            Assert.AreEqual(UserProfile.Employee, userResponse.Profile, "User should be reactivated as Employee");

            // Verify in database
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var reactivatedUser = await db.Users.FindAsync(inactiveEmployeeId);

                Assert.IsNotNull(reactivatedUser);
                Assert.AreEqual(UserProfile.Employee, reactivatedUser.Profile, "User should be Employee in database");
            }
        }

        [TestMethod]
        public async Task GetAll_ShouldIncludeInactiveUsers_WhenIncludeInactiveIsTrue()
        {
            // Arrange - Create an inactive user
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var inactiveUser = new User
                {
                    Name = "Usuário Inativo",
                    Email = "inactive@teste.com",
                    CompanyId = _testCompany.Id,
                    Profile = UserProfile.Inactive
                };
                inactiveUser.PasswordHash = PasswordHelper.Hash("Senha123", inactiveUser);
                db.Users.Add(inactiveUser);
                await db.SaveChangesAsync();
            }

            var request = new HttpRequestMessage(HttpMethod.Get, "/api/coworkers?includeInactive=true");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var listOfUsers = JsonConvert.DeserializeObject<List<UserLightDto>>(responseBody);

            Assert.IsNotNull(listOfUsers, "Response should not be null");
            
            // Should include the inactive user
            Assert.IsTrue(listOfUsers.Any(u => u.Email == "inactive@teste.com"), 
                "Should include inactive user when includeInactive=true");
            Assert.IsTrue(listOfUsers.Any(u => u.Profile == UserProfile.Inactive), 
                "Should include users with Inactive profile");
        }

        [TestMethod]
        public async Task CreateEmployee_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            // Arrange - Try to create employee with existing email
            var createDto = new CreateEmployeeDto
            {
                Name = "Duplicado",
                Email = "employee@teste.com", // Already exists
                ProfileType = UserProfile.Employee
            };

            var content = new StringContent(JsonConvert.SerializeObject(createDto), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/coworkers")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _adminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody);

            Assert.IsNotNull(apiResponse, "Response should not be null");
            Assert.IsFalse(apiResponse.Success, "Success should be false for duplicate email");
        }

        [TestMethod]
        public async Task Deactivate_ShouldReturnForbid_WhenEmployeeTriesToDeactivate()
        {
            // Arrange
            int adminId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                adminId = db.Users.First(u => u.Email == "admin@teste.com").Id;
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"/api/coworkers/{adminId}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody);

            Assert.IsNotNull(apiResponse);
            Assert.IsFalse(apiResponse.Success);
            StringAssert.Contains(apiResponse.Message, "administradores", "Message should mention administrators");
        }

        [TestMethod]
        public async Task Reactivate_ShouldReturnForbid_WhenEmployeeTriesToReactivate()
        {
            // Arrange - Create inactive user
            int inactiveUserId;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var inactiveUser = new User
                {
                    Name = "Inativo",
                    Email = "inativo2@teste.com",
                    CompanyId = _testCompany.Id,
                    Profile = UserProfile.Inactive
                };
                inactiveUser.PasswordHash = PasswordHelper.Hash("Senha123", inactiveUser);
                db.Users.Add(inactiveUser);
                await db.SaveChangesAsync();
                inactiveUserId = inactiveUser.Id;
            }

            var request = new HttpRequestMessage(HttpMethod.Post, $"/api/coworkers/{inactiveUserId}/reactivate");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody);

            Assert.IsNotNull(apiResponse);
            Assert.IsFalse(apiResponse.Success);
            StringAssert.Contains(apiResponse.Message, "administradores", "Message should mention administrators");
        }
    }
}
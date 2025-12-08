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
    public class CompanyControllerTests
    {
        private static CustomWebApplicationFactory _factory = null!;
        private static HttpClient _client = null!;
        private static string _systemAdminToken = null!;
        private static string _employeeToken = null!;
        private static Company _testCompany = null!;

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
            TestDataCleanup.ClearUsers(db);
            TestDataCleanup.ClearCompany(db);

            // Create test company
            _testCompany = new Company
            {
                Name = "Empresa Teste",
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
                Email = "admin@teste.com",
                CompanyId = _testCompany.Id,
                Profile = UserProfile.SystemAdmin
            };
            systemAdmin.PasswordHash = PasswordHelper.Hash("Senha123", systemAdmin);
            db.Users.Add(systemAdmin);

            // Create Employee user
            var employee = new User
            {
                Name = "Funcionário",
                Email = "user@teste.com",
                CompanyId = _testCompany.Id,
                Profile = UserProfile.Employee
            };
            employee.PasswordHash = PasswordHelper.Hash("Senha123", employee);
            db.Users.Add(employee);

            await db.SaveChangesAsync();

            // Get tokens for both users
            _systemAdminToken = await GetAuthToken("admin@teste.com", "Senha123");
            _employeeToken = await GetAuthToken("user@teste.com", "Senha123");
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
        public async Task GetById_ShouldReturnForbid_WhenUserIsNotAdminOrSysAdmin()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/company/{_testCompany.Id}");
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
            Assert.IsNotNull(apiResponse.Errors, "Errors collection should not be null");
            Assert.IsTrue(apiResponse.Errors.Count > 0, "Errors should contain the forbidden message");
        }

        [TestMethod]
        public async Task GetById_ShouldReturnOk_WhenUserIsSystemAdmin()
        {
            // Arrange
            var request = new HttpRequestMessage(HttpMethod.Get, $"/api/company/{_testCompany.Id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _systemAdminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var companyDto = JsonConvert.DeserializeObject<CompanyDto>(responseBody);

            Assert.IsNotNull(companyDto);
            Assert.AreEqual(_testCompany.Name, companyDto.Name);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnForbid_WhenUserIsNotAuthenticated()
        {
            // Arrange - no authorization header

            // Act
            var response = await _client.GetAsync($"/api/company/{_testCompany.Id}");

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [TestMethod]
        public async Task Update_ShouldReturnForbid_WhenUserNotAdmin()
        {
            // Arrange - Create a second company with its own employee
            Company secondCompany;
            string secondEmployeeToken;
            
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                // Create second company
                secondCompany = new Company
                {
                    Name = "Segunda Empresa",
                    CNPJ = "34028316000103", // Valid CNPJ
                    Address = new Address
                    {
                        Street = "Rua Secundária",
                        Number = "456",
                        Neighborhood = "Bairro Novo",
                        City = "Rio de Janeiro",
                        State = "RJ",
                        ZipCode = "20000-000"
                    },
                    Phone = new Phone
                    {
                        Mobile = "21988888888"
                    }
                };
                db.Companies.Add(secondCompany);
                await db.SaveChangesAsync();

                // Create employee for second company
                var secondEmployee = new User
                {
                    Name = "Funcionário Empresa 2",
                    Email = "employee2@teste.com",
                    CompanyId = secondCompany.Id,
                    Profile = UserProfile.Employee
                };
                secondEmployee.PasswordHash = PasswordHelper.Hash("Senha123", secondEmployee);
                db.Users.Add(secondEmployee);
                await db.SaveChangesAsync();
            }

            // Get token for second employee
            secondEmployeeToken = await GetAuthToken("employee2@teste.com", "Senha123");

            // Try to update the first company (test company) with employee from first company
            var updateDto = new UpdateCompanyDto
            {
                Name = "Empresa Atualizada",
                Address = new AddressDto
                {
                    Street = "Rua Nova",
                    Number = "456",
                    Neighborhood = "Centro",
                    City = "São Paulo",
                    State = "SP",
                    ZipCode = "01000-000"
                },
                Phone = new PhoneDto
                {
                    Mobile = "11888888888"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(updateDto), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/company/{_testCompany.Id}")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _employeeToken);

            // Act - Employee from first company tries to update first company
            var response = await _client.SendAsync(request);

            // Assert - Should be forbidden
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);

            // Validate ApiResponse structure
            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody);

            Assert.IsNotNull(apiResponse, "API response should not be null");
            Assert.IsFalse(apiResponse.Success, "Success should be false for forbidden access");
            Assert.IsNotNull(apiResponse.Message, "Message should not be null");
            StringAssert.Contains(apiResponse.Message, "não autorizado", "Message should mention unauthorized access");
            Assert.IsNotNull(apiResponse.Errors, "Errors collection should not be null");
            Assert.IsTrue(apiResponse.Errors.Count > 0, "Errors should contain the forbidden message");

            // Act - Employee from second company tries to update first company
            var request2 = new HttpRequestMessage(HttpMethod.Put, $"/api/company/{_testCompany.Id}")
            {
                Content = new StringContent(JsonConvert.SerializeObject(updateDto), Encoding.UTF8, "application/json")
            };
            request2.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secondEmployeeToken);

            var response2 = await _client.SendAsync(request2);

            // Assert - Should also be forbidden (employee from different company)
            Assert.AreEqual(HttpStatusCode.Forbidden, response2.StatusCode);

            // Validate second response
            var responseBody2 = await response2.Content.ReadAsStringAsync();
            var apiResponse2 = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody2);

            Assert.IsNotNull(apiResponse2, "API response 2 should not be null");
            Assert.IsFalse(apiResponse2.Success, "Success should be false for cross-company access");
            StringAssert.Contains(apiResponse2.Message, "não autorizado", "Message should mention unauthorized access");

            // Verify that neither company was updated
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var unchangedCompany = await db.Companies.FindAsync(_testCompany.Id);
                
                Assert.IsNotNull(unchangedCompany);
                Assert.AreEqual("Empresa Teste", unchangedCompany.Name); // Original name unchanged
                Assert.AreEqual("71142891000169", unchangedCompany.CNPJ); // Original CNPJ unchanged
            }
        }

        [TestMethod]
        public async Task Update_ShouldReturnBadRequest_WhenDtoIsInvalid()
        {
            // Arrange
            var invalidDto = new UpdateCompanyDto
            {
                Name = "", // Invalid - required field
                Address = new AddressDto
                {
                    Street = "Rua Teste",
                    Number = "123",
                    Neighborhood = "Centro",
                    City = "São Paulo",
                    State = "SP",
                    ZipCode = "01000-000"
                },
                Phone = new PhoneDto
                {
                    Mobile = "11999999999"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(invalidDto), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/company/{_testCompany.Id}")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _systemAdminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody);

            // Validate ApiResponse structure
            Assert.IsNotNull(apiResponse, "API response should not be null");
            Assert.IsFalse(apiResponse.Success, "Success should be false for validation errors");
            Assert.IsNotNull(apiResponse.Message, "Message should not be null");
            Assert.IsNotNull(apiResponse.Errors, "Errors collection should not be null");
            Assert.IsTrue(apiResponse.Errors.Count > 0, "Errors collection should contain at least one error");

            // Validate error message content
            StringAssert.Contains(apiResponse.Message, "validação", "Message should mention validation");

            // Validate that the specific validation error is in the Errors list
            Assert.IsTrue(apiResponse.Errors.Any(e => e.Contains("nome da empresa") || e.Contains("obrigatório")),
                "Errors should contain validation message about required company name");
        }

        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenCompanyNotExists()
        {
            // Arrange
            var validDto = new UpdateCompanyDto
            {
                Name = "Empresa Teste",
                Address = new AddressDto
                {
                    Street = "Rua Teste",
                    Number = "123",
                    Neighborhood = "Centro",
                    City = "São Paulo",
                    State = "SP",
                    ZipCode = "01000-000"
                },
                Phone = new PhoneDto
                {
                    Mobile = "11999999999"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(validDto), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, "/api/company/99999")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _systemAdminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public async Task Update_ShouldReturnOk_WhenSystemAdminUpdatesSuccessfully()
        {
            // Arrange
            var validDto = new UpdateCompanyDto
            {
                Name = "Empresa Atualizada",
                CNPJ = "06990590000123", // Valid CNPJ
                Address = new AddressDto
                {
                    Street = "Rua Nova",
                    Number = "456",
                    Complement = "Sala 10",
                    Neighborhood = "Vila Nova",
                    City = "Rio de Janeiro",
                    State = "RJ",
                    ZipCode = "20000-000"
                },
                Phone = new PhoneDto
                {
                    Mobile = "21987654321",
                    Landline = "2133334444"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(validDto), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/company/{_testCompany.Id}")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _systemAdminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var companyDto = JsonConvert.DeserializeObject<CompanyDto>(responseBody);

            Assert.IsNotNull(companyDto);
            Assert.AreEqual("Empresa Atualizada", companyDto.Name);
            Assert.AreEqual("06990590000123", companyDto.CNPJ);
            Assert.AreEqual("Rua Nova", companyDto.Address.Street);
            Assert.AreEqual("456", companyDto.Address.Number);
            Assert.AreEqual("21987654321", companyDto.Phone.Mobile);

            // Verify in database
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var updatedCompany = await db.Companies.FindAsync(_testCompany.Id);

            Assert.IsNotNull(updatedCompany);
            Assert.AreEqual("Empresa Atualizada", updatedCompany.Name);
            Assert.AreEqual("06990590000123", updatedCompany.CNPJ);
        }

        [TestMethod]
        public async Task Update_ShouldReturnOk_WhenAdminUpdatesOwnCompany()
        {
            // Arrange - Create an Admin user (not SystemAdmin)
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var admin = new User
                {
                    Name = "Admin",
                    Email = "admin2@teste.com",
                    CompanyId = _testCompany.Id,
                    Profile = UserProfile.Admin
                };
                admin.PasswordHash = PasswordHelper.Hash("Senha123", admin);
                db.Users.Add(admin);
                await db.SaveChangesAsync();
            }

            var adminToken = await GetAuthToken("admin2@teste.com", "Senha123");

            var validDto = new UpdateCompanyDto
            {
                Name = "Empresa Admin Updated",
                Address = new AddressDto
                {
                    Street = "Rua Admin",
                    Number = "789",
                    Neighborhood = "Centro",
                    City = "São Paulo",
                    State = "SP",
                    ZipCode = "01000-000"
                },
                Phone = new PhoneDto
                {
                    Mobile = "11955555555"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(validDto), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/company/{_testCompany.Id}")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var responseBody = await response.Content.ReadAsStringAsync();
            var companyDto = JsonConvert.DeserializeObject<CompanyDto>(responseBody);

            Assert.IsNotNull(companyDto);
            Assert.AreEqual("Empresa Admin Updated", companyDto.Name);
        }

        [TestMethod]
        public async Task Update_ShouldReturnForbid_WhenAdminTriesToUpdateDifferentCompany()
        {
            // Arrange - Create another company and an admin for it
            Company anotherCompany;
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                anotherCompany = new Company
                {
                    Name = "Outra Empresa",
                    CNPJ = "34028316000103" // Valid CNPJ
                };
                db.Companies.Add(anotherCompany);
                await db.SaveChangesAsync();

                var adminOtherCompany = new User
                {
                    Name = "Admin Outra Empresa",
                    Email = "adminother@teste.com",
                    CompanyId = anotherCompany.Id,
                    Profile = UserProfile.Admin
                };
                adminOtherCompany.PasswordHash = PasswordHelper.Hash("Senha123", adminOtherCompany);
                db.Users.Add(adminOtherCompany);
                await db.SaveChangesAsync();
            }

            var adminOtherToken = await GetAuthToken("adminother@teste.com", "Senha123");

            var validDto = new UpdateCompanyDto
            {
                Name = "Tentativa de Update",
                Address = new AddressDto
                {
                    Street = "Rua Teste",
                    Number = "123",
                    Neighborhood = "Centro",
                    City = "São Paulo",
                    State = "SP",
                    ZipCode = "01000-000"
                },
                Phone = new PhoneDto
                {
                    Mobile = "11999999999"
                }
            };

            var content = new StringContent(JsonConvert.SerializeObject(validDto), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Put, $"/api/company/{_testCompany.Id}")
            {
                Content = content
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminOtherToken);

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);

            // Validate ApiResponse structure
            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(responseBody);

            Assert.IsNotNull(apiResponse, "API response should not be null");
            Assert.IsFalse(apiResponse.Success, "Success should be false for cross-company access");
            Assert.IsNotNull(apiResponse.Message, "Message should not be null");
            StringAssert.Contains(apiResponse.Message, "não autorizado", "Message should mention unauthorized access");
            Assert.IsNotNull(apiResponse.Errors, "Errors collection should not be null");
            Assert.IsTrue(apiResponse.Errors.Count > 0, "Errors should contain the forbidden message");
            
            // Verify the company was not modified
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var unchangedCompany = await db.Companies.FindAsync(_testCompany.Id);
                
                Assert.IsNotNull(unchangedCompany);
                Assert.AreEqual("Empresa Teste", unchangedCompany.Name);
                Assert.AreNotEqual("Tentativa de Update", unchangedCompany.Name);
            }
        }
    }
}
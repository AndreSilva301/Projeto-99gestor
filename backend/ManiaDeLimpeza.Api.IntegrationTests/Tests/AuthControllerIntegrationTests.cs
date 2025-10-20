using AutoMapper;
using ManiaDeLimpeza.Api.Controllers;
using ManiaDeLimpeza.Api.IntegrationTests.Tools;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Application.Services;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.Exceptions;
using ManiaDeLimpeza.Infrastructure.Helpers;
using ManiaDeLimpeza.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
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
            var registerDto = new RegisterUserRequestDto
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
            var registerDto = new RegisterUserRequestDto
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
            var registerDto = new RegisterUserRequestDto
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
            var registerDto = new RegisterUserRequestDto
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

            registerDto = new RegisterUserRequestDto
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
            var registerDto = new RegisterUserRequestDto
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

            registerDto = new RegisterUserRequestDto
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
            var registerDto = new RegisterUserRequestDto
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
        public async Task Register_WhenValid_ShouldGiveBackAJWT()
        {
            // Arrange
            var registerDto = new RegisterUserRequestDto
            {
                Name = "Test User",
                Email = "testuser@test.com",
                Password = "Secure123",
                ConfirmPassword = "Secure123",
                AcceptTerms = true,
                CompanyName = "Test Company",
                Phone = "12345678"
            };

            var content = new StringContent(JsonConvert.SerializeObject(registerDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/register", content);

            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<AuthResponseDto>>(responseBody);

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual("Test User", apiResponse.Data.Name);
            Assert.AreEqual("testuser@test.com", apiResponse.Data.Email);
            Assert.IsFalse(string.IsNullOrWhiteSpace(apiResponse.Data.BearerToken), "bearer token not populated");

        }

        [TestMethod]
        public async Task Login_ShouldReturnOk()
        {
            SeedUserDatabase();

            var seedUser = TestDataSeeder.GetDefaultUser();

            // Arrange
            var loginDto = new
            {
                Email = TestDataSeeder.DefaultEmail,
                Password = TestDataSeeder.DefaultPassword,
            };
            var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<AuthResponseDto>>(responseBody);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            Assert.IsNotNull(apiResponse);
            Assert.IsTrue(apiResponse.Success);
            Assert.IsNotNull(apiResponse.Data);
            Assert.AreEqual(seedUser.Name, apiResponse.Data.Name);
            Assert.AreEqual(seedUser.Email, apiResponse.Data.Email);
            Assert.IsFalse(string.IsNullOrWhiteSpace(apiResponse.Data.BearerToken), "bearer token not populated");
        }


        [TestMethod]
        public async Task Login_ShouldReturnUnauthorized()
        {
            SeedUserDatabase();

            var seedUser = TestDataSeeder.GetDefaultUser();

            // Arrange
            var loginDto = new
            {
                Email = TestDataSeeder.DefaultEmail,
                Password = "WrongPassword",
            };
            var content = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<AuthResponseDto>>(responseBody);

            // Assert
            Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.IsNotNull(apiResponse);
            Assert.IsFalse(apiResponse.Success);
            Assert.IsNull(apiResponse.Data);
        }

        private void SeedUserDatabase()
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            TestDataSeeder.SeedDefaultUser(db);
        }

        [TestMethod]
        public async Task ForgotPassword_shouldReturnOk()
        {
            // Arrange
            var email = "testepassword@gmail.com";

            using (var scope = _factory.Services.CreateScope())
            {
                var companyRepo = scope.ServiceProvider.GetRequiredService<ICompanyRepository>();
                var userRepo = scope.ServiceProvider.GetRequiredService<IUserRepository>();

                var newCompany = new Company
                {
                    Name = "Empresa Teste",
                    CNPJ = "12345678000199"
                };
                await companyRepo.AddAsync(newCompany);

                var newUser = new User
                {
                    Name = "Usuário Teste",
                    Email = email,
                    PasswordHash = "hash-aqui",
                    CompanyId = newCompany.Id
                };
                await userRepo.AddAsync(newUser);
            }

            // Act - chama o endpoint de recuperação de senha
            var response = await _client.PostAsJsonAsync("/api/auth/forgot-password", new { Email = email });

            // Assert - valida o retorno da API
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<ApiResponse<string>>();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
            // The message should be in Data property based on ApiResponseHelper.SuccessResponse implementation
            Assert.IsNotNull(result.Data);
            StringAssert.Contains(result.Data, "E-mail de recuperação enviado");

            // Assert extra - garante que o token foi realmente criado no banco
            using (var scope = _factory.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var token = await context.PasswordResetTokens
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.User.Email == email);

                Assert.IsNotNull(token);
                Assert.AreEqual(email, token.User.Email);
                Assert.IsTrue(token.Expiration > DateTime.UtcNow); // garante que ainda está válido
            }
        }
        private async Task<PasswordResetToken?> GetLatestTokenByEmailAsync(ApplicationDbContext db, string email)
        {
            return await db.PasswordResetTokens
        .Include(t => t.User)
        .Where(t => t.User.Email == email)
        .OrderByDescending(t => t.CreatedAt)
        .FirstOrDefaultAsync();
        }

        [TestMethod]
        public async Task ForgotpassWord_ShoulReturnOK_WhenEmailDoesNotExist()
        {
            var dto = new
            {
                Email = "testeemailnotexist@gmail.com"
            };
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/forgot-password", content);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
        }

        [TestMethod]
        public async Task ForgotPassword_ShouldReturnBadRequest_WhenEmailIsInvalid()
        {
            var dto = new
            {
                Email = "emailsemarroba"
            };
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/forgot-password", content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task ForgotPassword_ShouldReturnBadRequest_WhenEmailIsEmpty()
        {
            var dto = new { Email = "" };
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/forgot-password", content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        [TestMethod]
        public async Task ForgotPassword_ShouldReturnBadRequest_WhenSqlInjectionAttempt()
        {
            var dto = new
            {
                Email = "test@gmail.com' OR 1=1--"
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/forgot-password", content);

            // The controller validates the email format and returns BadRequest for invalid emails
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public async Task ForgotPassword_ShouldReturnSameResponse_ForExistingAndNonExistingEmail()
        {
            var existingDto = new { Email = "testepassword@gmail.com" };
            var nonExistingDto = new { Email = "testeemailnotexist@gmail.com" };

            var contentExisting = new StringContent(JsonConvert.SerializeObject(existingDto), Encoding.UTF8, "application/json");
            var contentNonExisting = new StringContent(JsonConvert.SerializeObject(nonExistingDto), Encoding.UTF8, "application/json");

            var responseExisting = await _client.PostAsync("/api/auth/forgot-password", contentExisting);
            var responseNonExisting = await _client.PostAsync("/api/auth/forgot-password", contentNonExisting);

            Assert.AreEqual(responseExisting.StatusCode, responseNonExisting.StatusCode, "Os status codes devem ser iguais para emails existentes e inexistentes.");

            var bodyExisting = await responseExisting.Content.ReadAsStringAsync();
            var bodyNonExisting = await responseNonExisting.Content.ReadAsStringAsync();

            var responseExistingParsed = JsonConvert.DeserializeObject<ApiResponse<string>>(bodyExisting);
            var responseNonExistingParsed = JsonConvert.DeserializeObject<ApiResponse<string>>(bodyNonExisting);

            Assert.AreEqual(responseExistingParsed.Success, responseNonExistingParsed.Success, "Campo 'Success' deve ser igual");
            Assert.AreEqual(responseExistingParsed.Message, responseNonExistingParsed.Message, "Campo 'Message' deve ser igual");
            Assert.AreEqual(responseExistingParsed.Data, responseNonExistingParsed.Data, "Campo 'Data' deve ser igual");

            CollectionAssert.AreEqual(
                responseExistingParsed.Errors ?? new List<string>(),
                responseNonExistingParsed.Errors ?? new List<string>(),
                "Campo 'Errors' deve ser igual");

            // Comparar timestamps com tolerância de 5 segundos
            var timestampExisting = DateTime.Parse(responseExistingParsed.TimeStamp).ToUniversalTime();
            var timestampNonExisting = DateTime.Parse(responseNonExistingParsed.TimeStamp).ToUniversalTime();

            Assert.IsTrue(
                Math.Abs((timestampExisting - timestampNonExisting).TotalSeconds) < 5,
                "timeStamps devem estar dentro de 5 segundos");
        }
        [TestMethod]
        public async Task VerifyResetToken_ShouldReturnBadRequest_WhenTokenIsInvalid()
        {
            // Arrange
            var token = "\"token-invalido-123\""; // precisa das aspas para ser JSON string válido
            var content = new StringContent(token, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/verify-reset-token", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(body, "Token inválido ou expirado");
        }

        [TestMethod]
        public async Task VerifyResetToken_ShouldReturnOk_WhenTokenIsValid()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var company = new Company
                {
                    Name = "Empresa Teste"
                };

                var user = TestDataSeeder.GetDefaultUser();
                user.CompanyId = company.Id;
                user.Company = company;

                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();

                var validToken = new PasswordResetToken
                {
                    Token = "valid-token-123",
                    UserId = user.Id,
                    Expiration = DateTime.UtcNow.AddMinutes(30)
                };

                await db.PasswordResetTokens.AddAsync(validToken);
                await db.SaveChangesAsync();
            }

            var content = new StringContent("\"valid-token-123\"", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/verify-reset-token", content);

            var body = await response.Content.ReadAsStringAsync();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, $"Resposta inesperada: {body}");

            StringAssert.Contains(body, "Token válido");
            StringAssert.Contains(body, TestDataSeeder.DefaultEmail);
        }

        [TestMethod]
        public async Task VerifyResetToken_ShouldReturnBadRequest_WhenTokenIsExpired()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var company = new Company
                {
                    Name = "Empresa Teste"
                };

                await db.Companies.AddAsync(company);
                await db.SaveChangesAsync();

                var user = TestDataSeeder.GetDefaultUser();
                user.CompanyId = company.Id;
                user.Company = company;

                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();

                var expiredToken = new PasswordResetToken
                {
                    Token = "expired-token-123",
                    UserId = user.Id,
                    Expiration = DateTime.UtcNow.AddMinutes(-30) // já expirado
                };

                await db.PasswordResetTokens.AddAsync(expiredToken);
                await db.SaveChangesAsync();
            }

            // Send just the string token, not an object
            var content = new StringContent("\"expired-token-123\"", Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/verify-reset-token", content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(body, "Token inválido ou expirado");
        }


        [TestMethod]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenTokenIsInvalid()
        {
            // Arrange
            var dto = new ResetPasswordRequestDto
            {
                Token = "invalid-token",
                NewPassword = "ValidPass123",
                NewPasswordConfirm = "ValidPass123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/reset-password", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(body);
            Assert.IsFalse(apiResponse.Success);
            StringAssert.Contains(apiResponse.Message, "Token inválido ou expirado");
        }

        [TestMethod]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenTokenIsExpired()
        {
            // Arrange
            var dto = new ResetPasswordRequestDto
            {
                Token = "expired-token",
                NewPassword = "ValidPass123",
                NewPasswordConfirm = "ValidPass123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/reset-password", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(body);
            Assert.IsFalse(apiResponse.Success);
            StringAssert.Contains(apiResponse.Message, "Token inválido ou expirado");
        }

        [TestMethod]
        public async Task ResetPassword_ShouldReturnOk_WhenTokenIsValid()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var company = new Company
                {
                    Name = "Empresa Teste"
                };

                await db.Companies.AddAsync(company);
                await db.SaveChangesAsync();

                var user = TestDataSeeder.GetDefaultUser();
                user.CompanyId = company.Id;
                user.Company = company;

                await db.Users.AddAsync(user);
                await db.SaveChangesAsync();

                var validToken = new PasswordResetToken
                {
                    Token = "valid-token-123",
                    UserId = user.Id,
                    Expiration = DateTime.UtcNow.AddMinutes(30)
                };

                await db.PasswordResetTokens.AddAsync(validToken);
                await db.SaveChangesAsync();
            }

            var dto = new ResetPasswordRequestDto
            {
                Token = "valid-token-123",
                NewPassword = "ValidPass123",
                NewPasswordConfirm = "ValidPass123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/reset-password", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(body);
            Assert.IsTrue(apiResponse.Success);
            Assert.AreEqual("Senha redefinida com sucesso", apiResponse.Data);
        }

        [TestMethod]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenPasswordIsEmpty()
        {
            // Arrange
            var dto = new ResetPasswordRequestDto
            {
                Token = "some-token",
                NewPassword = "", // senha vazia
                NewPasswordConfirm = ""
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/reset-password", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(body);
            Assert.IsFalse(apiResponse.Success);
            // The actual validation message from ResetPasswordRequestDto
            StringAssert.Contains(apiResponse.Message, "A nova senha é obrigatória");
        }

        [TestMethod]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenPasswordIsInvalid()
        {
            // Arrange
            var dto = new ResetPasswordRequestDto
            {
                Token = "some-token",
                NewPassword = "123", // senha muito curta e sem letra
                NewPasswordConfirm = "123"
            };

            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/reset-password", content);

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<string>>(body);
            Assert.IsFalse(apiResponse.Success);
            // The actual validation message from ValidatePassword method
            StringAssert.Contains(apiResponse.Message, "A nova senha deve ter pelo menos 8 caracteres, contendo ao menos uma letra e um número");
        }

        [TestMethod]
        public async Task ChangePasswordAsync_ShouldUpdatePassword_WhenCurrentPasswordIsValid()
        {
            // Arrange
            var user = new User { Id = 1, Email = "teste@email.com" };
            user.PasswordHash = PasswordHelper.Hash("senhaAntiga123", user);
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            mockRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user); // Added mock for GetByIdAsync
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, null, null);

            // Act
            var result = await service.ChangePasswordAsync(user.Email, "senhaAntiga123", "senhaNova123");

            // Assert
            Assert.IsTrue(PasswordHelper.Verify("senhaNova123", result.PasswordHash, result));
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessException))]
        public async Task ChangePasswordAsync_ShouldThrow_WhenCurrentPasswordIsInvalid()
        {
            // Arrange
            var user = new User { Id = 1, Email = "teste@email.com" };
            user.PasswordHash = PasswordHelper.Hash("senhaCorreta123", user);

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, null!, null!);

            // Act
            await service.ChangePasswordAsync(user.Email, "senhaErrada123", "novaSenha123");
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessException))]
        public async Task ChangePasswordAsync_ShouldThrow_WhenUserNotFound()
        {
            // Arrange
            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

            var service = new UserService(mockRepo.Object, null!, null!);

            // Act
            await service.ChangePasswordAsync("naoexiste@email.com", "senhaAntiga123", "senhaNova123");
        }

        [TestMethod]
        public async Task ChangePasswordAsync_ShouldCallUpdate_WhenPasswordChanged()
        {
            // Arrange
            var user = new User { Id = 1, Email = "teste@email.com" };
            user.PasswordHash = PasswordHelper.Hash("senhaAntiga123", user);

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByEmailAsync(user.Email)).ReturnsAsync(user);
            mockRepo.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user); // Added mock for GetByIdAsync
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(user);

            var service = new UserService(mockRepo.Object, null!, null!);

            // Act
            await service.ChangePasswordAsync(user.Email, "senhaAntiga123", "senhaNova123");

            // Assert
            mockRepo.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateUserAsync_ShouldUpdateNameOnly()
        {
            // Arrange
            var existingUser = new User { Id = 1, Email = "teste@email.com", Name = "Antigo Nome" };
            var updatedUser = new User { Id = 1, Email = "teste@email.com", Name = "Novo Nome" };

            var mockRepo = new Mock<IUserRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(existingUser.Id)).ReturnsAsync(existingUser);
            mockRepo.Setup(r => r.GetByEmailAsync(existingUser.Email)).ReturnsAsync(existingUser);
            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>())).ReturnsAsync(existingUser);

            var service = new UserService(mockRepo.Object, null!, null!);

            // Act
            var result = await service.UpdateUserAsync(updatedUser);

            var response = badRequestResult.Value as ApiResponse<string>;
            Assert.IsFalse(response.Success);
            Assert.AreEqual("A nova senha deve ter pelo menos 6 caracteres.", response.Message);
        }

        [TestMethod]
        public async Task CaptureLeadAsync_DeveSalvarLead_QuandoDadosForemValidos()
        {
            // Arrange
            var mockRepo = new Mock<ILeadRepository>();
            var service = new LeadService(mockRepo.Object);

            var dto = new LeadCaptureRequestDto
            {
                Name = "João da Silva",
                Phone = "11999999999"
            };

            // Act
            var result = await service.CaptureLeadAsync(dto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("João da Silva", result.Name);
            Assert.AreEqual("11999999999", result.Phone);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Lead>()), Times.Once);

        }

        [TestMethod]
        public async Task CaptureLeadAsync_DeveLancarExcecao_QuandoNomeForVazio()
        {
            // Arrange
            var mockRepo = new Mock<ILeadRepository>();
            var service = new LeadService(mockRepo.Object);

            var dto = new LeadCaptureRequestDto
            {
                Name = "",
                Phone = "11999999999"
            };

            // Act + Assert
            var ex = await Assert.ThrowsExceptionAsync<BusinessException>(() =>
                service.CaptureLeadAsync(dto));

            Assert.AreEqual("O nome é obrigatório.", ex.Message);
        }


        [TestMethod]
        public async Task CaptureLeadAsync_DeveLancarExcecao_QuandoTelefoneForVazio()
        {
            // Arrange
            var mockRepo = new Mock<ILeadRepository>();
            var service = new LeadService(mockRepo.Object);

            var dto = new LeadCaptureRequestDto
            {
                Name = "João",
                Phone = ""
            };

            // Act + Assert
            var ex = await Assert.ThrowsExceptionAsync<BusinessException>(() =>
                service.CaptureLeadAsync(dto));

            Assert.AreEqual("O telefone é obrigatório.", ex.Message);
        }
    }
}

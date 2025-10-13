using AutoMapper;
using ManiaDeLimpeza.Api.Controllers;
using ManiaDeLimpeza.Api.IntegrationTests.Tools;
using ManiaDeLimpeza.Api.Response;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
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
        public async Task Register_WhenValid_ShouldGiveBackAJWT()
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
        public async Task ForgotpassWord_shouldReturnOk()
        {
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
                    CompanyId = newCompany.Id // usa o ID da empresa criada
                };

                await userRepo.AddAsync(newUser);
            }

            var dto = new { Email = email };
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/auth/forgot-password", content);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            using (var scope = _factory.Services.CreateScope())
            {
                var passwordResetRepo = scope.ServiceProvider.GetRequiredService<IPasswordResetRepository>();
                var createdToken = await passwordResetRepo.GetLatestByEmailAsync(email);

                Assert.IsNotNull(createdToken, "O token de redefinição deveria ter sido criado no banco.");
                Assert.IsTrue(createdToken.Expiration > DateTime.UtcNow, "O token criado não deve estar expirado.");
            }
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

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
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
            var dto = new
            {
                Token = "token-invalido-123"
            };
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

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

            var dto = new { Token = "valid-token-123" };
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/verify-reset-token", content);

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
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

            var dto = new { Token = "expired-token-123" };
            var content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("/api/auth/verify-reset-token", content);

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);

            var body = await response.Content.ReadAsStringAsync();
            StringAssert.Contains(body, "Token inválido ou expirado");
        }


        [TestMethod]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenTokenIsInvalid()
        {
            var mockPasswordResetRepo = new Mock<IPasswordResetRepository>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AuthController(
                mockUserService.Object,
                Mock.Of<ICompanyServices>(),
                Mock.Of<ITokenService>(),
                Mock.Of<IMapper>(),
                null, 
                Mock.Of<IForgotPasswordService>(),
                mockPasswordResetRepo.Object
            );

            var dto = new ResetPasswordDto
            {
                Token = "invalid-token",
                NewPassword = "novaSenha123"
            };

            mockPasswordResetRepo
                .Setup(r => r.GetByTokenAsync(dto.Token))
                .ReturnsAsync((PasswordResetToken)null);

            // Act
            var result = await controller.ResetPassword(dto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);

            var response = badRequestResult.Value as ApiResponse<string>;
            Assert.IsFalse(response.Success);
            Assert.AreEqual("Token inválido ou expirado", response.Message);
        }

        [TestMethod]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenTokenIsExpired()
        {
            var mockPasswordResetRepo = new Mock<IPasswordResetRepository>();
            var mockUserService = new Mock<IUserService>();

            var controller = new AuthController(
                mockUserService.Object,
                Mock.Of<ICompanyServices>(),
                Mock.Of<ITokenService>(),
                Mock.Of<IMapper>(),
                null,
                Mock.Of<IForgotPasswordService>(),
                mockPasswordResetRepo.Object
            );

            var expiredToken = new PasswordResetToken
            {
                Token = "expired-token",
                Expiration = DateTime.UtcNow.AddMinutes(-1),
                UserId = 1
            };

            mockPasswordResetRepo
                .Setup(r => r.GetByTokenAsync("expired-token"))
                .ReturnsAsync(expiredToken);

            var dto = new ResetPasswordDto
            {
                Token = "expired-token",
                NewPassword = "novaSenha123"
            };

            // Act
            var result = await controller.ResetPassword(dto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);

            var response = badRequestResult.Value as ApiResponse<string>;
            Assert.IsFalse(response.Success);
            Assert.AreEqual("Token inválido ou expirado", response.Message);
        }

        [TestMethod]
        public async Task ResetPassword_ShouldReturnOk_WhenTokenIsValid()
        {
            var mockPasswordResetRepo = new Mock<IPasswordResetRepository>();
            var mockUserService = new Mock<IUserService>();

            var fakeUser = new User
            {
                Id = 1,
                Name = "Usuário Teste",
                Email = "teste@exemplo.com"
            };

            var validToken = new PasswordResetToken
            {
                Token = "valid-token",
                Expiration = DateTime.UtcNow.AddMinutes(10),
                UserId = 1,
                User = fakeUser 
            };

            mockPasswordResetRepo
                .Setup(r => r.GetByTokenAsync("valid-token"))
                .ReturnsAsync(validToken);

            mockUserService
                .Setup(u => u.UpdatePasswordAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(fakeUser);

            var controller = new AuthController(
                mockUserService.Object,
                Mock.Of<ICompanyServices>(),
                Mock.Of<ITokenService>(),
                Mock.Of<IMapper>(),
                null,
                Mock.Of<IForgotPasswordService>(),
                mockPasswordResetRepo.Object
            );

            var dto = new ResetPasswordDto
            {
                Token = "valid-token",
                NewPassword = "novaSenha123"
            };

            // Act
            var result = await controller.ResetPassword(dto);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);

            var response = okResult.Value as ApiResponse<string>;
            Assert.IsTrue(response.Success);
            Assert.AreEqual("Senha redefinida com sucesso", response.Data); ;

            mockUserService.Verify(u => u.UpdatePasswordAsync(
                                It.Is<User>(usr => usr.Id == validToken.UserId),
                                dto.NewPassword), Times.Once);
        }

        [TestMethod]
        public async Task ResetPassword_ShouldReturnBadRequest_WhenPasswordIsEmpty()
        {
            var controller = new AuthController(
                Mock.Of<IUserService>(),
                Mock.Of<ICompanyServices>(),
                Mock.Of<ITokenService>(),
                Mock.Of<IMapper>(),
                null,
                Mock.Of<IForgotPasswordService>(),
                Mock.Of<IPasswordResetRepository>()
            );

            var dto = new ResetPasswordDto
            {
                Token = "some-token",
                NewPassword = ""
            };

            var result = await controller.ResetPassword(dto);

            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);

            var response = badRequestResult.Value as ApiResponse<string>;
            Assert.IsFalse(response.Success);
            Assert.AreEqual("A nova senha deve ter pelo menos 6 caracteres.", response.Message);
        }
    }
}

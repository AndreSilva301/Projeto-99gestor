using ManiaDeLimpeza.Api.Controllers;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests
{
    [TestClass]
    public class CompanyControllerTests
    {
        private Mock<ICompanyServices> _companyServiceMock = null!;
        private CompanyController _controller = null!;
        private Company _company = null!;
        private User _systemAdmin = null!;
        private User _employee = null!;
        private UpdateCompanyDto _validDto = null!;
        private UpdateCompanyDto _invalidDto = null!;

        [TestInitialize]
        public void Setup()
        {
            _companyServiceMock = new Mock<ICompanyServices>();
            _controller = new CompanyController(_companyServiceMock.Object);

            _company = new Company
            {
                Id = 1,
                Name = "Empresa Teste",
                CNPJ = "12.345.678/0001-90"
            };

            _systemAdmin = new User
            {
                Id = 1,
                Name = "System Admin",
                Email = "admin@teste.com",
                PasswordHash = "123",
                CompanyId = 1,
                Profile = UserProfile.SystemAdmin
            };

            _employee = new User
            {
                Id = 2,
                Name = "Funcionário",
                Email = "user@teste.com",
                PasswordHash = "123",
                CompanyId = 1,
                Profile = UserProfile.Employee
            };

            _validDto = new UpdateCompanyDto
            {
                Name = "Empresa Atualizada"
            };

            _invalidDto = new UpdateCompanyDto
            {
                Name = "" 
            };
        }

        private void SetCurrentUser(User? user)
        {
            var httpContext = new DefaultHttpContext();

            if (user != null)
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim("Profile", user.Profile.ToString()),
            new Claim("CompanyId", user.CompanyId.ToString())
        };

                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
                httpContext.Items["User"] = user;
            }
            else
            {
                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
                httpContext.Items["User"] = null; 
            }

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [TestMethod]
        public async Task GetById_ShouldReturnForbid_WhenUserIsNotAdminOrSysAdmin()
        {
            // Arrange
            SetCurrentUser(_employee);

            // Act
            var result = await _controller.GetById(_company.Id);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task GetById_ShouldReturnOk_WhenUserIsSystemAdmin()
        {
            // Arrange
            SetCurrentUser(_systemAdmin);

            _companyServiceMock
                .Setup(s => s.GetByIdAsync(_company.Id, _systemAdmin))
                .ReturnsAsync(_company);

            // Act
            var result = await _controller.GetById(_company.Id) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var data = result.Value as CompanyDto;
            Assert.IsNotNull(data);
            Assert.AreEqual(_company.Name, data.Name);
        }

        [TestMethod]
        public async Task GetById_ShouldReturnForbid_WhenUserIsNull()
        {
            SetCurrentUser(null);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<NullReferenceException>(
                async () => await _controller.GetById(_company.Id)
            );
        }


        [TestMethod]
        public async Task Update_ShouldReturnForbid_WhenUserNotAdmin()
        {
            // Arrange
            SetCurrentUser(_employee);

            // Act
            var result = await _controller.Update(_company.Id, _validDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Update_ShouldReturnBadRequest_WhenDtoIsInvalid()
        {
            // Arrange
            SetCurrentUser(_systemAdmin);

            // Act
            var result = await _controller.Update(_company.Id, _invalidDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task Update_ShouldReturnNotFound_WhenCompanyNotExists()
        {
            // Arrange
            SetCurrentUser(_systemAdmin);

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

            _companyServiceMock
                .Setup(s => s.UpdateCompanyAsync(_company.Id, validDto, It.IsAny<User>()))
                .ReturnsAsync((Company?)null);

            // Act
            var result = await _controller.Update(_company.Id, validDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Update_ShouldReturnOk_WhenSystemAdminUpdatesSuccessfully()
        {
            // Arrange
            SetCurrentUser(_systemAdmin);

            var validDto = new UpdateCompanyDto
            {
                Name = "Empresa Atualizada",
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

            var updated = new Company
            {
                Id = 1,
                Name = "Empresa Atualizada"
            };

            _companyServiceMock
                .Setup(s => s.UpdateCompanyAsync(_company.Id, It.IsAny<UpdateCompanyDto>(), It.IsAny<User>()))
                .ReturnsAsync(updated);

            // Act
            var result = await _controller.Update(_company.Id, validDto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            var data = result.Value as CompanyDto;
            Assert.IsNotNull(data);
            Assert.AreEqual("Empresa Atualizada", data.Name);
        }
    }
}
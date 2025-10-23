using AutoMapper;
using ManiaDeLimpeza.Api.Controllers;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests
{
    [TestClass]
    public class CoworkersControllerTests
    {
        private Mock<IUserService> _userServiceMock = null!;
        private Mock<IMapper> _mapperMock = null!;
        private CoworkersController _controller = null!;
        private User _adminUser = null!;
        private User _employeeUser = null!;
        private CreateEmployeeDto _createDto = null!;
        private UpdateUserDto _updateDto = null!;

        [TestInitialize]
        public void Setup()
        {
            _userServiceMock = new Mock<IUserService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new CoworkersController(_userServiceMock.Object, _mapperMock.Object);

            _adminUser = new User
            {
                Id = 1,
                Name = "Admin User",
                Email = "admin@empresa.com",
                CompanyId = 10,
                Profile = UserProfile.Admin
            };

            _employeeUser = new User
            {
                Id = 2,
                Name = "Colaborador",
                Email = "user@empresa.com",
                CompanyId = 10,
                Profile = UserProfile.Employee
            };

            _createDto = new CreateEmployeeDto
            {
                Name = "Novo Colaborador",
                Email = "novo@empresa.com",
                ProfileType = UserProfile.Employee
            };

            _updateDto = new UpdateUserDto
            {
                Name = "Colaborador Atualizado",
                Email = "atualizado@empresa.com"
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
        public async Task GetAll_ShouldReturnForbid_WhenUserIsNotAdmin()
        {
            SetCurrentUser(_employeeUser);

            var result = await _controller.GetAll();

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task GetAll_ShouldReturnOk_WhenAdminRequests()
        {
            // Arrange
            var _adminUser = new User
            {
                Id = 1,
                Name = "System Admin", 
                Email = "admin@empresa.com",
                Profile = UserProfile.SystemAdmin, 
                CompanyId = 10
            };

            SetCurrentUser(_adminUser);

            var users = new List<User>
            {
        new User { Id = 2, Name = "João", Email = "joao@empresa.com", Profile = UserProfile.Employee, CompanyId = 10 }
            };

            _userServiceMock
                .Setup(s => s.GetUsersByCompanyIdAsync(_adminUser.CompanyId, false))
                .ReturnsAsync(users);

            var mapped = new List<UserListDto>
            {
        new UserListDto { Id = 2, Name = "João", Email = "joao@empresa.com", Profile = UserProfile.Employee }
            };

            _mapperMock
                .Setup(m => m.Map<IEnumerable<UserListDto>>(It.IsAny<IEnumerable<User>>()))
                .Returns(mapped);

            // Act
            var result = await _controller.GetAll() as OkObjectResult;

            // Assert
            Assert.IsNotNull(result, "O método não retornou OkObjectResult — possivelmente retornou ForbidResult.");
            Assert.AreEqual(200, result.StatusCode);

            var data = result.Value as IEnumerable<UserListDto>;
            Assert.IsNotNull(data);
            Assert.AreEqual(1, data.Count());
            Assert.AreEqual("João", data.First().Name);
        }

        [TestMethod]
        public async Task CreateEmployee_ShouldReturnForbid_WhenNotAdmin()
        {
            SetCurrentUser(_employeeUser);

            var result = await _controller.CreateEmployee(_createDto);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task CreateEmployee_ShouldReturnBadRequest_WhenProfileIsSystemAdmin()
        {
            // Arrange
            var systemAdmin = new User
            {
                Id = 1,
                Name = "SysAdmin",
                Email = "admin@empresa.com",
                Profile = UserProfile.SystemAdmin, 
                CompanyId = 10
            };

            SetCurrentUser(systemAdmin);

            var invalidDto = new CreateEmployeeDto
            {
                Name = "SysAdmin",
                Email = "sys@empresa.com",
                ProfileType = UserProfile.SystemAdmin
            };

            // Act
            var result = await _controller.CreateEmployee(invalidDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
        }

        [TestMethod]
        public async Task CreateEmployee_ShouldReturnOk_WhenCreatedSuccessfully()
        {
            // Arrange
            _adminUser.Profile = UserProfile.SystemAdmin;
            SetCurrentUser(_adminUser);

            var createdUser = new User
            {
                Id = 3,
                Name = "Novo Colaborador",
                Email = "novo@empresa.com",
                CompanyId = 10,
                Profile = UserProfile.Employee
            };

            _userServiceMock
                .Setup(s => s.CreateEmployeeAsync(_createDto.Name, _createDto.Email, _adminUser.CompanyId))
                .ReturnsAsync(createdUser);

            _mapperMock
                .Setup(m => m.Map<UserListDto>(createdUser))
                .Returns(new UserListDto
                {
                    Id = createdUser.Id,
                    Name = createdUser.Name,
                    Email = createdUser.Email,
                    Profile = createdUser.Profile
                });

            // Act
            var actionResult = await _controller.CreateEmployee(_createDto);
            var result = actionResult as OkObjectResult;

            // Assert
            Assert.IsNotNull(result, "Esperava-se um OkObjectResult, mas o resultado foi nulo (provavelmente um Forbid ou erro interno).");
            Assert.AreEqual(200, result.StatusCode, "O código de status deveria ser 200 (OK).");

            var data = result.Value as UserListDto;
            Assert.IsNotNull(data, "O corpo da resposta deveria conter um UserListDto.");
            Assert.AreEqual("Novo Colaborador", data.Name);
            Assert.AreEqual("novo@empresa.com", data.Email);
            Assert.AreEqual(UserProfile.Employee, data.Profile);
        }

        [TestMethod]
        public async Task Update_ShouldReturnForbid_WhenUserNotAllowed()
        {
            SetCurrentUser(_employeeUser);

            var result = await _controller.Update(999, _updateDto);

            Assert.IsInstanceOfType(result, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Deactivate_ShouldReturnOk_WhenAdminDeactivatesUser()
        {
            // Arrange
            _adminUser.Profile = UserProfile.SystemAdmin;
            _adminUser.CompanyId = 10;
            SetCurrentUser(_adminUser);

            var user = new User { Id = 2, CompanyId = 10, Profile = UserProfile.Employee, Name = "Colaborador", Email = "user@empresa.com" };

            _userServiceMock.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(user);
            _userServiceMock.Setup(s => s.DeactivateUserAsync(2)).ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<UserListDto>(It.IsAny<User>()))
                       .Returns((User u) => new UserListDto { Id = u.Id, Name = u.Name, Email = u.Email, Profile = u.Profile });

            // Act
            var actionResult = await _controller.Deactivate(2);
            var result = actionResult as OkObjectResult;

            // Assert
            Assert.IsNotNull(result, "Esperava OkObjectResult (provavelmente veio Forbid ou Unauthorized).");
            Assert.AreEqual(200, result.StatusCode);

            var dto = result.Value as UserListDto;
            Assert.IsNotNull(dto);
            Assert.AreEqual(2, dto.Id);
            Assert.AreEqual("Colaborador", dto.Name);
        }

        [TestMethod]
        public async Task Reactivate_ShouldReturnOk_WhenAdminReactivatesUser()
        {
            // Arrange
            _adminUser.Profile = UserProfile.SystemAdmin; 
            _adminUser.CompanyId = 10;                    
            SetCurrentUser(_adminUser);

            var user = new User
            {
                Id = 2,
                CompanyId = 10,
                Profile = UserProfile.Employee,
                Name = "Colaborador",
                Email = "user@empresa.com"
            };

            _userServiceMock.Setup(s => s.GetByIdAsync(2)).ReturnsAsync(user);
            _userServiceMock.Setup(s => s.ReactivateUserAsync(2)).ReturnsAsync(user);

            _mapperMock.Setup(m => m.Map<UserListDto>(It.IsAny<User>()))
                       .Returns((User u) => new UserListDto
                       {
                           Id = u.Id,
                           Name = u.Name,
                           Email = u.Email,
                           Profile = u.Profile
                       });

            // Act
            var actionResult = await _controller.Reactivate(2);
            var result = actionResult as OkObjectResult;

            // Assert
            Assert.IsNotNull(result, "Esperava OkObjectResult — provavelmente veio Forbid ou Unauthorized.");
            Assert.AreEqual(200, result.StatusCode);

            var dto = result.Value as UserListDto;
            Assert.IsNotNull(dto);
            Assert.AreEqual(2, dto.Id);
            Assert.AreEqual("Colaborador", dto.Name);
        }
    }
}
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Api.Controllers.Base;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Application.Dtos.Mappers;
using Microsoft.AspNetCore.Http;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tests;
[TestClass]
public class CompanyControllerTests
{
    private Mock<ICompanyServices> _companyServiceMock;
    private CompanyController _controller;
    private User _systemAdmin;
    private User _companyAdmin;
    private User _unauthorizedUser;
    private Company _company;

    [TestInitialize]
    public void Setup()
    {
        _companyServiceMock = new Mock<ICompanyServices>();

        _controller = new CompanyController(_companyServiceMock.Object)
        {
            ControllerContext = new ControllerContext()
        };

        _systemAdmin = new User { Id = 1, Profile = UserProfile.SystemAdmin };
        _companyAdmin = new User { Id = 2, Profile = UserProfile.SystemAdmin, CompanyId = 100 };
        _unauthorizedUser = new User { Id = 3, Profile = UserProfile.Employee, CompanyId = 200 };

        _company = new Company
        {
            Id = 100,
            Name = "Test Company",
            CNPJ = "123456789",
            Address = new Address
            {
                Street = "Rua A",
                Number = "10",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                ZipCode = "12345-000"
            },
            Phone = new Phone { Mobile = "99999-9999" }
        };
    }

    private void SetCurrentUser(User user)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Items["User"] = user;
        _controller.ControllerContext.HttpContext = httpContext;
    }

    [TestMethod]
    public async Task GetAll_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
    {
        SetCurrentUser(null);

        var result = await _controller.GetAll();

        Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    public async Task GetAll_ShouldReturnForbid_WhenUserIsNotSystemAdmin()
    {
        SetCurrentUser(_unauthorizedUser);

        var result = await _controller.GetAll();

        Assert.IsInstanceOfType(result, typeof(ForbidResult));
    }

    [TestMethod]
    public async Task GetAll_ShouldReturnCompanies_WhenSystemAdmin()
    {
        SetCurrentUser(_systemAdmin);

        _companyServiceMock.Setup(s => s.GetAllAsync(_systemAdmin))
            .ReturnsAsync(new List<Company> { _company });

        var result = await _controller.GetAll() as OkObjectResult;

        Assert.IsNotNull(result);
        var data = result.Value as IEnumerable<CompanyDto>;
        Assert.IsNotNull(data);
        Assert.AreEqual(1, data.Count());
    }

    [TestMethod]
    public async Task GetById_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
    {
        SetCurrentUser(null);

        var result = await _controller.GetById(100);

        Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    public async Task GetById_ShouldReturnForbid_WhenUserIsNotAdminOrSystemAdmin()
    {
        SetCurrentUser(_unauthorizedUser);

        var result = await _controller.GetById(100);

        Assert.IsInstanceOfType(result, typeof(ForbidResult));
    }

    [TestMethod]
    public async Task GetById_ShouldReturnNotFound_WhenCompanyDoesNotExist()
    {
        SetCurrentUser(_systemAdmin);

        _companyServiceMock.Setup(s => s.GetByIdAsync(999, _systemAdmin))
            .ReturnsAsync((Company?)null);

        var result = await _controller.GetById(999);

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task GetById_ShouldReturnCompany_WhenAuthorized()
    {
        SetCurrentUser(_companyAdmin);

        _companyServiceMock.Setup(s => s.GetByIdAsync(100, _companyAdmin))
            .ReturnsAsync(_company);

        var result = await _controller.GetById(100) as OkObjectResult;

        Assert.IsNotNull(result);
        var dto = result.Value as CompanyDto;
        Assert.IsNotNull(dto);
        Assert.AreEqual(_company.Id, dto.Id);
    }

    [TestMethod]
    public async Task Update_ShouldReturnUnauthorized_WhenUserNotAuthenticated()
    {
        SetCurrentUser(null);

        var result = await _controller.Update(100, new UpdateCompanyDto());

        Assert.IsInstanceOfType(result, typeof(UnauthorizedObjectResult));
    }

    [TestMethod]
    public async Task Update_ShouldReturnForbid_WhenUserIsNotAuthorized()
    {
        SetCurrentUser(_unauthorizedUser);

        var result = await _controller.Update(100, new UpdateCompanyDto());

        Assert.IsInstanceOfType(result, typeof(ForbidResult));
    }

    [TestMethod]
    public async Task Update_ShouldReturnBadRequest_WhenAddressIsNull()
    {
        SetCurrentUser(_systemAdmin);
        var dto = new UpdateCompanyDto { Address = null };

        var result = await _controller.Update(100, dto);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Update_ShouldReturnBadRequest_WhenAddressIsIncomplete()
    {
        SetCurrentUser(_systemAdmin);
        var dto = new UpdateCompanyDto { Address = new AddressDto() };

        var result = await _controller.Update(100, dto);

        Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
    }

    [TestMethod]
    public async Task Update_ShouldReturnNotFound_WhenCompanyDoesNotExist()
    {
        SetCurrentUser(_systemAdmin);

        var dto = new UpdateCompanyDto
        {
            Name = "Test",
            CNPJ = "123",
            Address = new AddressDto
            {
                Street = "Rua A",
                Number = "1",
                Neighborhood = "Centro",
                City = "São Paulo",
                State = "SP",
                ZipCode = "12345-000"
            },
            Phone = new PhoneDto()
        };

        _companyServiceMock.Setup(s => s.GetByIdAsync(100, _systemAdmin))
            .ReturnsAsync((Company?)null);

        var result = await _controller.Update(100, dto);

        Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
    }

    [TestMethod]
    public async Task Update_ShouldReturnOk_WhenUpdateIsSuccessful()
    {
        SetCurrentUser(_systemAdmin);

        var dto = new UpdateCompanyDto
        {
            Name = "Updated Company",
            CNPJ = "9999999",
            Address = new AddressDto
            {
                Street = "Nova Rua",
                Number = "123",
                Neighborhood = "Centro",
                City = "SP",
                State = "SP",
                ZipCode = "00000-000"
            },
            Phone = new PhoneDto { Mobile = "88888-8888" }
        };

        _companyServiceMock.Setup(s => s.GetByIdAsync(100, _systemAdmin))
            .ReturnsAsync(_company);
        _companyServiceMock.Setup(s => s.UpdateCompanyAsync(It.IsAny<Company>()))
            .ReturnsAsync(_company);

        var result = await _controller.Update(100, dto) as OkObjectResult;

        Assert.IsNotNull(result);
        var updated = result.Value as CompanyDto;
        Assert.IsNotNull(updated);
        Assert.AreEqual("Updated Company", updated.Name);
    }
}
using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Services;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Exceptions;
using ManiaDeLimpeza.Domain.Persistence;
using Moq;

namespace ManiaDeLimpeza.Application.UnitTests.Services;
[TestClass]
public class CustomerServiceTests
{
    private Mock<ICustomerRepository> _customerRepositoryMock = null!;
    private Mock<IUserRepository> _userRepositoryMock = null!;
    private Mock<IMapper> _mapperMock = null!;
    private CustomerService _customerService = null!;
    [TestInitialize]
    public void Setup()
    {
        _customerRepositoryMock = new Mock<ICustomerRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();

        _customerService = new CustomerService(
            _customerRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object
        );
    }

    [TestMethod]
    public async Task CreateAsync_DeveCriarClienteComSucesso()
    {
        // Arrange
        var dto = new CustomerCreateDto
        {
            Name = "Cliente Teste",
            Email = "cliente@teste.com", 
            Phone = new PhoneDto
            {
                Mobile = "11999999999",  
                Landline = "1133334444"  
            }
        };
        var user = new User { Id = 1, CompanyId = 10 };
        var entity = new Customer { Id = 99, CompanyId = 10, Name = "Cliente Teste" };
        var mappedDto = new CustomerDto { Id = 99, Name = "Cliente Teste" };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<Customer>(dto)).Returns(entity);
        _mapperMock.Setup(m => m.Map<CustomerDto>(entity)).Returns(mappedDto);

        // Act
        var result = await _customerService.CreateAsync(dto, user.CompanyId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Cliente Teste", result.Name);
        _customerRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Customer>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessException))]
    public async Task CreateAsync_DeveLancarExcecao_QuandoDtoInvalido()
    {
        // Arrange
        var dto = new CustomerCreateDto();
        var user = new User { Id = 1, CompanyId = 10 };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

        // Act
        await _customerService.CreateAsync(dto, user.Id);
    }

    [TestMethod]
    public async Task UpdateAsync_DeveAtualizarClienteComSucesso()
    {
        // Arrange
        var user = new User { Id = 1, CompanyId = 10 };
        var existing = new Customer { Id = 1, CompanyId = 10, Name = "Antigo" };
        var dto = new CustomerUpdateDto
        {
            Name = "Novo Nome",
            Email = "cliente@teste.com",
            Phone = new PhoneDto { Mobile = "11999999999" },
        };
        var mappedDto = new CustomerDto { Id = 1, Name = "Novo Nome" };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _customerRepositoryMock.Setup(r => r.GetbyIdWithRelationshipAsync(existing.Id)).ReturnsAsync(existing);
        _mapperMock.Setup(m => m.Map(dto, existing)).Callback<CustomerUpdateDto, Customer>((src, dest) => dest.Name = src.Name);
        _mapperMock.Setup(m => m.Map<CustomerDto>(existing)).Returns(mappedDto);

        // Act
        var result = await _customerService.UpdateAsync(existing.Id, dto, user.CompanyId);

        // Assert
        Assert.AreEqual("Novo Nome", result.Name);
        _customerRepositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessException))]
    public async Task UpdateAsync_DeveFalhar_ClienteDeOutraEmpresa()
    {
        // Arrange
        var user = new User { Id = 1, CompanyId = 10 };
        var existing = new Customer { Id = 5, CompanyId = 99 };
        var dto = new CustomerUpdateDto { Name = "Teste" };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _customerRepositoryMock.Setup(r => r.GetbyIdWithRelationshipAsync(existing.Id)).ReturnsAsync(existing);

        // Act
        await _customerService.UpdateAsync(existing.Id, dto, user.Id);
    }

    [TestMethod]
    public async Task SoftDeleteAsync_DeveExecutarComSucesso()
    {
        // Arrange
        var user = new User { Id = 1, CompanyId = 10 };
        var customer = new Customer { Id = 3, CompanyId = 10 };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _customerRepositoryMock.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

        // Act
        await _customerService.SoftDeleteAsync(customer.Id, user.CompanyId);

        // Assert
        _customerRepositoryMock.Verify(r => r.SoftDeleteAsync(customer.Id), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessException))]
    public async Task SoftDeleteAsync_DeveFalhar_ClienteDeOutraEmpresa()
    {
        // Arrange
        var user = new User { Id = 1, CompanyId = 10 };
        var customer = new Customer { Id = 5, CompanyId = 99 };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
        _customerRepositoryMock.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

        // Act
        await _customerService.SoftDeleteAsync(customer.Id, user.Id);
    }
}
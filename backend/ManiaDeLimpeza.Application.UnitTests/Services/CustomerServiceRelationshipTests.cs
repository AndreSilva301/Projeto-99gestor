using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Services;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Exceptions;
using ManiaDeLimpeza.Domain.Persistence;
using Moq;

namespace ManiaDeLimpeza.Application.UnitTests.Services;
[TestClass]
public class CustomerServiceRelationshipTests
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
    public async Task AddOrUpdateRelationshipsAsync_ShouldCreateNewRelationships_WhenValid()
    {
        var companyId = 1;
        var customerId = 10;
        var customer = new Customer { Id = customerId, CompanyId = companyId };

        var dtos = new List<CustomerRelationshipDto>
            {
                new CustomerRelationshipDto
                {
                    Id = 0,
                    Description = "Visita técnica"
                }
            };

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);

        _mapperMock.Setup(m => m.Map<CustomerRelationship>(It.IsAny<CustomerRelationshipDto>()))
            .Returns((CustomerRelationshipDto dto) => new CustomerRelationship
            {
                Id = dto.Id,
                Description = dto.Description
            });

        _customerRepositoryMock.Setup(r => r.AddOrUpdateRelationshipsAsync(customerId, It.IsAny<IEnumerable<CustomerRelationship>>()))
            .ReturnsAsync((int id, IEnumerable<CustomerRelationship> rels) => rels);

        _mapperMock.Setup(m => m.Map<CustomerRelationshipDto>(It.IsAny<CustomerRelationship>()))
            .Returns((CustomerRelationship rel) => new CustomerRelationshipDto
            {
                Id = rel.Id,
                Description = rel.Description
            });

        var result = await _customerService.AddOrUpdateRelationshipsAsync(customerId, dtos, companyId);

        Assert.AreEqual(1, result.Count());
        _customerRepositoryMock.Verify(r => r.AddOrUpdateRelationshipsAsync(customerId, It.IsAny<IEnumerable<CustomerRelationship>>()), Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessException))]
    public async Task AddOrUpdateRelationshipsAsync_ShouldThrow_WhenCustomerDoesNotBelongToCompany()
    {
        var customer = new Customer { Id = 10, CompanyId = 99 };
        var dtos = new List<CustomerRelationshipDto> { new CustomerRelationshipDto { Id = 0, Description = "Teste" } };

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

        await _customerService.AddOrUpdateRelationshipsAsync(customer.Id, dtos, companyId: 1);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessException))]
    public async Task AddOrUpdateRelationshipsAsync_ShouldThrow_WhenInvalidRelationship()
    {
        var companyId = 1;
        var customer = new Customer { Id = 10, CompanyId = companyId };
        var dtos = new List<CustomerRelationshipDto>
            {
                new CustomerRelationshipDto { Id = 0, Description = "" } 
            };

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(customer.Id)).ReturnsAsync(customer);

        await _customerService.AddOrUpdateRelationshipsAsync(customer.Id, dtos, companyId);
    }

    [TestMethod]
    public async Task ListRelationshipsAsync_ShouldReturnOrderedList_WhenValid()
    {
        var companyId = 1;
        var customerId = 5;
        var customer = new Customer { Id = customerId, CompanyId = companyId };
        var relationships = new List<CustomerRelationship>
            {
                new CustomerRelationship { Id = 1, CustomerId = customerId, Description = "Antigo", DateTime = DateTime.UtcNow.AddDays(-1) },
                new CustomerRelationship { Id = 2, CustomerId = customerId, Description = "Recente", DateTime = DateTime.UtcNow }
            };

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);
        _customerRepositoryMock.Setup(r => r.GetRelationshipsByCustomerAsync(customerId))
            .ReturnsAsync(relationships);

        _mapperMock.Setup(m => m.Map<CustomerRelationshipDto>(It.IsAny<CustomerRelationship>()))
            .Returns((CustomerRelationship rel) => new CustomerRelationshipDto
            {
                Id = rel.Id,
                Description = rel.Description
            });

        var result = await _customerService.ListRelationshipsAsync(customerId, companyId);

        Assert.AreEqual(2, result.Count());
        Assert.AreEqual("Recente", result.First().Description);
    }

    [TestMethod]
    [ExpectedException(typeof(BusinessException))]
    public async Task DeleteRelationshipsAsync_ShouldThrow_WhenRelationshipDoesNotBelongToCustomer()
    {
        var companyId = 1;
        var customerId = 7;
        var customer = new Customer { Id = customerId, CompanyId = companyId };
        var relationships = new List<CustomerRelationship>
            {
                new CustomerRelationship { Id = 1, CustomerId = customerId }
            };

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);
        _customerRepositoryMock.Setup(r => r.GetRelationshipsByCustomerAsync(customerId))
            .ReturnsAsync(relationships);

        var invalidIds = new List<int> { 99 };

        await _customerService.DeleteRelationshipsAsync(customerId, invalidIds, companyId);
    }

    [TestMethod]
    public async Task DeleteRelationshipsAsync_ShouldExecute_WhenValid()
    {
        var companyId = 1;
        var customerId = 7;
        var customer = new Customer { Id = customerId, CompanyId = companyId };
        var relationships = new List<CustomerRelationship>
            {
                new CustomerRelationship { Id = 1, CustomerId = customerId },
                new CustomerRelationship { Id = 2, CustomerId = customerId }
            };

        _customerRepositoryMock.Setup(r => r.GetByIdAsync(customerId))
            .ReturnsAsync(customer);
        _customerRepositoryMock.Setup(r => r.GetRelationshipsByCustomerAsync(customerId))
            .ReturnsAsync(relationships);

        var idsToDelete = new List<int> { 1 };

        await _customerService.DeleteRelationshipsAsync(customerId, idsToDelete, companyId);

        _customerRepositoryMock.Verify(r => r.DeleteRelationshipsAsync(idsToDelete, customerId), Times.Once);
    }
}
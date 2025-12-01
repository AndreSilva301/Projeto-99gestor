using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Services;
using ManiaDeLimpeza.Application.UnitTests.Tools;
using ManiaDeLimpeza.Domain;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Exceptions;
using ManiaDeLimpeza.Domain.Persistence;
using Moq;

namespace ManiaDeLimpeza.Application.UnitTests.Services
{
    [TestClass]
    public class QuoteServiceTests : TestsBase
    {
        private Mock<IQuoteRepository> _quoteRepositoryMock;
        private Mock<ICustomerRepository> _customerRepositoryMock;
        private Mock<IMapper> _mapperMock;
        private QuoteService _quoteService;

        [TestInitialize]
        public void Setup()
        {
            _quoteRepositoryMock = new Mock<IQuoteRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _mapperMock = new Mock<IMapper>();

            _quoteService = new QuoteService(
                _quoteRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _mapperMock.Object
            );

        }

        #region CreateAsync Tests

        [TestMethod]
        [ExpectedException(typeof(BusinessException))]
        public async Task CreateAsync_WhenCustomerDoesNotExist_ShouldThrowBusinessException()
        {
            // Arrange
            var dto = new CreateQuoteDto
            {
                CustomerId = 999,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<QuoteItemDto>
                {
                    new QuoteItemDto { Description = "Test", Quantity = 1, UnitPrice = 100 }
                }
            };

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(dto.CustomerId))
                .ReturnsAsync((Customer?)null);

            // Act
            await _quoteService.CreateAsync(dto, userId: 1, companyId: 1);

            // Assert - Exception expected
        }

        [TestMethod]
        public async Task CreateAsync_WhenCustomerDoesNotExist_ShouldThrowExceptionWithCorrectMessage()
        {
            // Arrange
            var dto = new CreateQuoteDto
            {
                CustomerId = 999,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<QuoteItemDto>
                {
                    new QuoteItemDto { Description = "Test", Quantity = 1, UnitPrice = 100 }
                }
            };

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(dto.CustomerId))
                .ReturnsAsync((Customer?)null);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<BusinessException>(
                () => _quoteService.CreateAsync(dto, userId: 1, companyId: 1)
            );

            Assert.AreEqual("Cliente não pertence à sua empresa.", exception.Message);
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessException))]
        public async Task CreateAsync_WhenCustomerBelongsToDifferentCompany_ShouldThrowBusinessException()
        {
            // Arrange
            var dto = new CreateQuoteDto
            {
                CustomerId = 1,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<QuoteItemDto>
                {
                    new QuoteItemDto { Description = "Test", Quantity = 1, UnitPrice = 100 }
                }
            };

            var customer = new Customer
            {
                Id = 1,
                CompanyId = 999, // Different company
                Name = "Test Customer"
            };

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(dto.CustomerId))
                .ReturnsAsync(customer);

            // Act
            await _quoteService.CreateAsync(dto, userId: 1, companyId: 1);

            // Assert - Exception expected
        }

        [TestMethod]
        public async Task CreateAsync_WithValidData_ShouldSetCorrectUserId()
        {
            // Arrange
            var userId = 42;
            var companyId = 1;
            var dto = new CreateQuoteDto
            {
                CustomerId = 1,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<QuoteItemDto>
                {
                    new QuoteItemDto { Description = "Test", Quantity = 1, UnitPrice = 100 }
                }
            };

            var customer = new Customer { Id = 1, CompanyId = companyId, Name = "Test" };
            Quote capturedQuote = null!;

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(dto.CustomerId))
                .ReturnsAsync(customer);

            _mapperMock
                .Setup(x => x.Map<Quote>(dto))
                .Returns(new Quote
                {
                    CustomerId = dto.CustomerId,
                    QuoteItems = new List<QuoteItem>
                    {
                        new QuoteItem { Description = "Test", Quantity = 1, UnitPrice = 100 }
                    }
                });

            _quoteRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => capturedQuote = q)
                .ReturnsAsync((Quote q) => q);

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.CreateAsync(dto, userId, companyId);

            // Assert
            Assert.IsNotNull(capturedQuote);
            Assert.AreEqual(userId, capturedQuote.UserId);
        }

        [TestMethod]
        public async Task CreateAsync_WithValidData_ShouldSetCorrectCompanyId()
        {
            // Arrange
            var userId = 1;
            var companyId = 99;
            var dto = new CreateQuoteDto
            {
                CustomerId = 1,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<QuoteItemDto>
                {
                    new QuoteItemDto { Description = "Test", Quantity = 1, UnitPrice = 100 }
                }
            };

            var customer = new Customer { Id = 1, CompanyId = companyId, Name = "Test" };
            Quote capturedQuote = null!;

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(dto.CustomerId))
                .ReturnsAsync(customer);

            _mapperMock
                .Setup(x => x.Map<Quote>(dto))
                .Returns(new Quote
                {
                    CustomerId = dto.CustomerId,
                    QuoteItems = new List<QuoteItem>
                    {
                        new QuoteItem { Description = "Test", Quantity = 1, UnitPrice = 100 }
                    }
                });

            _quoteRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => capturedQuote = q)
                .ReturnsAsync((Quote q) => q);

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.CreateAsync(dto, userId, companyId);

            // Assert
            Assert.IsNotNull(capturedQuote);
            Assert.AreEqual(companyId, capturedQuote.CompanyId);
        }

        [TestMethod]
        public async Task CreateAsync_WithValidData_ShouldSetCreatedAtToUtcNow()
        {
            // Arrange
            var beforeCreate = DateTime.UtcNow;
            var dto = new CreateQuoteDto
            {
                CustomerId = 1,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<QuoteItemDto>
                {
                    new QuoteItemDto { Description = "Test", Quantity = 1, UnitPrice = 100 }
                }
            };

            var customer = new Customer { Id = 1, CompanyId = 1, Name = "Test" };
            Quote capturedQuote = null!;

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(dto.CustomerId))
                .ReturnsAsync(customer);

            _mapperMock
                .Setup(x => x.Map<Quote>(dto))
                .Returns(new Quote
                {
                    CustomerId = dto.CustomerId,
                    QuoteItems = new List<QuoteItem>
                    {
                        new QuoteItem { Description = "Test", Quantity = 1, UnitPrice = 100 }
                    }
                });

            _quoteRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => capturedQuote = q)
                .ReturnsAsync((Quote q) => q);

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.CreateAsync(dto, userId: 1, companyId: 1);
            var afterCreate = DateTime.UtcNow;

            // Assert
            Assert.IsNotNull(capturedQuote);
            Assert.IsTrue(capturedQuote.CreatedAt >= beforeCreate);
            Assert.IsTrue(capturedQuote.CreatedAt <= afterCreate);
            Assert.AreEqual(DateTimeKind.Utc, capturedQuote.CreatedAt.Kind);
        }

        [TestMethod]
        public async Task CreateAsync_WithValidData_ShouldCallRecalculateTotals()
        {
            // Arrange
            var dto = new CreateQuoteDto
            {
                CustomerId = 1,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<QuoteItemDto>
                {
                    new QuoteItemDto { Description = "Item 1", Quantity = 2, UnitPrice = 50 },
                    new QuoteItemDto { Description = "Item 2", Quantity = 1, UnitPrice = 100 }
                }
            };

            var customer = new Customer { Id = 1, CompanyId = 1, Name = "Test" };
            Quote capturedQuote = null!;

            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(dto.CustomerId))
                .ReturnsAsync(customer);

            _mapperMock
                .Setup(x => x.Map<Quote>(dto))
                .Returns(new Quote
                {
                    CustomerId = dto.CustomerId,
                    QuoteItems = new List<QuoteItem>
                    {
                        new QuoteItem { Description = "Item 1", Quantity = 2, UnitPrice = 50 },
                        new QuoteItem { Description = "Item 2", Quantity = 1, UnitPrice = 100 }
                    }
                });

            _quoteRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => capturedQuote = q)
                .ReturnsAsync((Quote q) => q);

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.CreateAsync(dto, userId: 1, companyId: 1);

            // Assert - TotalPrice should be calculated (2*50 + 1*100 = 200)
            Assert.IsNotNull(capturedQuote);
            Assert.AreEqual(200m, capturedQuote.TotalPrice);
        }

        #endregion

        #region GetByIdAsync Tests

        [TestMethod]
        public async Task GetByIdAsync_WhenQuoteDoesNotExist_ShouldReturnNull()
        {
            // Arrange
            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Quote?)null);

            // Act
            var result = await _quoteService.GetByIdAsync(999, companyId: 1);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenQuoteBelongsToDifferentCompany_ShouldReturnNull()
        {
            // Arrange
            var quote = new Quote
            {
                Id = 1,
                CompanyId = 999,
                CustomerId = 1,
                UserId = 1
            };

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(quote);

            // Act
            var result = await _quoteService.GetByIdAsync(1, companyId: 1);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetByIdAsync_WhenQuoteBelongsToCorrectCompany_ShouldReturnQuote()
        {
            // Arrange
            var quote = new Quote
            {
                Id = 1,
                CompanyId = 1,
                CustomerId = 1,
                UserId = 1
            };

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(quote);

            // Act
            var result = await _quoteService.GetByIdAsync(1, companyId: 1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Id);
        }

        #endregion

        #region UpdateAsync Tests

        [TestMethod]
        [ExpectedException(typeof(BusinessException))]
        public async Task UpdateAsync_WhenQuoteDoesNotExist_ShouldThrowBusinessException()
        {
            // Arrange
            var dto = new UpdateQuoteDto
            {   
                Id = 999,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<UpdateQuoteItemDto>()
            };

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Quote?)null);

            // Act
            await _quoteService.UpdateAsync(dto, companyId: 1);

            // Assert - Exception expected
        }

        [TestMethod]
        public async Task UpdateAsync_WhenQuoteDoesNotExist_ShouldThrowExceptionWithCorrectMessage()
        {
            // Arrange
            var dto = new UpdateQuoteDto
            {
                Id = 999,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<UpdateQuoteItemDto>()
            };

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(999))
                .ReturnsAsync((Quote?)null);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<BusinessException>(
                () => _quoteService.UpdateAsync(dto, companyId: 1)
            );

            Assert.AreEqual("Quote with id 999 not found.", exception.Message);
        }

        [TestMethod]
        [ExpectedException(typeof(BusinessException))]
        public async Task UpdateAsync_WhenQuoteBelongsToDifferentCompany_ShouldThrowBusinessException()
        {
            // Arrange
            var dto = new UpdateQuoteDto
            {   Id = 999,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<UpdateQuoteItemDto>()
            };

            var existingQuote = new Quote
            {
                Id = 1,
                CompanyId = 2, // Different company
                CustomerId = 1,
                UserId = 1,
                QuoteItems = new List<QuoteItem>()
            };

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existingQuote);

            // Act
            await _quoteService.UpdateAsync(dto, companyId: 1);

            // Assert - Exception expected
        }

        [TestMethod]
        public async Task UpdateAsync_WhenQuoteBelongsToDifferentCompany_ShouldThrowExceptionWithCorrectMessage()
        {
            // Arrange
            var dto = new UpdateQuoteDto
            {
                Id = 1,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<UpdateQuoteItemDto>()
            };

            var existingQuote = new Quote
            {
                Id = 1,
                CompanyId = 999,
                CustomerId = 1,
                UserId = 1,
                QuoteItems = new List<QuoteItem>()
            };

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existingQuote);

            // Act & Assert
            var exception = await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                () => _quoteService.UpdateAsync(dto, companyId: 1)
            );

            Assert.AreEqual("Quote does not belong to the company.", exception.Message);
        }

        [TestMethod]
        public async Task UpdateAsync_WithValidData_ShouldUpdatePaymentMethod()
        {
            // Arrange
            var dto = new UpdateQuoteDto
            {
                Id = 1,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentConditions = "3x",
                Items = new List<UpdateQuoteItemDto>()
            };

            var existingQuote = new Quote
            {
                Id = 1,
                CompanyId = 1,
                CustomerId = 1,
                UserId = 1,
                PaymentMethod = PaymentMethod.Cash,
                QuoteItems = new List<QuoteItem>()
            };

            Quote updatedQuote = null!;

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existingQuote);

            _quoteRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => updatedQuote = q)
                .ReturnsAsync((Quote q) => q);

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.UpdateAsync(dto, companyId: 1);

            // Assert
            Assert.IsNotNull(updatedQuote);
            Assert.AreEqual(PaymentMethod.CreditCard, updatedQuote.PaymentMethod);
        }

        [TestMethod]
        public async Task UpdateAsync_WithValidData_ShouldUpdatePaymentConditions()
        {
            // Arrange
            var dto = new UpdateQuoteDto
            {
                Id = 1,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentConditions = "3x sem juros",
                Items = new List<UpdateQuoteItemDto>()
            };

            var existingQuote = new Quote
            {
                Id = 1,
                CompanyId = 1,
                CustomerId = 1,
                UserId = 1,
                PaymentConditions = "À vista",
                QuoteItems = new List<QuoteItem>()
            };

            Quote updatedQuote = null!;

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existingQuote);

            _quoteRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => updatedQuote = q)
                .ReturnsAsync((Quote q) => q);

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.UpdateAsync(dto, companyId: 1);

            // Assert
            Assert.IsNotNull(updatedQuote);
            Assert.AreEqual("3x sem juros", updatedQuote.PaymentConditions);
        }

        [TestMethod]
        public async Task UpdateAsync_WithValidData_ShouldUpdateCashDiscount()
        {
            // Arrange
            var dto = new UpdateQuoteDto
            {
                Id = 1,
                PaymentMethod = PaymentMethod.Cash,
                CashDiscount = 50.00m,
                Items = new List<UpdateQuoteItemDto>()
            };

            var existingQuote = new Quote
            {
                Id = 1,
                CompanyId = 1,
                CustomerId = 1,
                UserId = 1,
                CashDiscount = 0,
                QuoteItems = new List<QuoteItem>()
            };

            Quote updatedQuote = null!;

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existingQuote);

            _quoteRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => updatedQuote = q)
                .ReturnsAsync((Quote q) => q);

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.UpdateAsync(dto, companyId: 1);

            // Assert
            Assert.IsNotNull(updatedQuote);
            Assert.AreEqual(50.00m, updatedQuote.CashDiscount);
        }

        [TestMethod]
        public async Task UpdateAsync_WithValidData_ShouldSetUpdatedAtToUtcNow()
        {
            // Arrange
            var beforeUpdate = DateTime.UtcNow;
            var dto = new UpdateQuoteDto
            {
                Id = 1,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<UpdateQuoteItemDto>()
            };

            var existingQuote = new Quote
            {
                Id = 1,
                CompanyId = 1,
                CustomerId = 1,
                UserId = 1,
                QuoteItems = new List<QuoteItem>()
            };

            Quote updatedQuote = null!;

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existingQuote);

            _quoteRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => updatedQuote = q)
                .ReturnsAsync((Quote q) => q);

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.UpdateAsync(dto, companyId: 1);
            var afterUpdate = DateTime.UtcNow;

            // Assert
            Assert.IsNotNull(updatedQuote);
            Assert.IsNotNull(updatedQuote.UpdatedAt);
            Assert.IsTrue(updatedQuote.UpdatedAt >= beforeUpdate);
            Assert.IsTrue(updatedQuote.UpdatedAt <= afterUpdate);
            Assert.AreEqual(DateTimeKind.Utc, updatedQuote.UpdatedAt.Value.Kind);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenRemovingItems_ShouldRemoveItemsNotInUpdateDto()
        {
            // Arrange
            var dto = new UpdateQuoteDto
            {
                Id = 1,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<UpdateQuoteItemDto>
                {
                    new UpdateQuoteItemDto { Id = 1, Description = "Keep", Quantity = 1, UnitPrice = 100 }
                    // Item with Id 2 is not included, so it should be removed
                }
            };

            var existingQuote = new Quote
            {
                Id = 1,
                CompanyId = 1,
                CustomerId = 1,
                UserId = 1,
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem { Id = 1, Description = "Keep", Quantity = 1, UnitPrice = 100 },
                    new QuoteItem { Id = 2, Description = "Remove", Quantity = 1, UnitPrice = 50 }
                }
            };

            Quote updatedQuote = null!;

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existingQuote);

            _quoteRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => updatedQuote = q)
                .ReturnsAsync((Quote q) => q);

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.UpdateAsync(dto, companyId: 1);

            // Assert
            Assert.IsNotNull(updatedQuote);
            Assert.AreEqual(1, updatedQuote.QuoteItems.Count);
            Assert.IsTrue(updatedQuote.QuoteItems.Any(i => i.Id == 1));
            Assert.IsFalse(updatedQuote.QuoteItems.Any(i => i.Id == 2));
        }

        [TestMethod]
        public async Task UpdateAsync_WhenUpdatingExistingItem_ShouldUpdateItemProperties()
        {
            // Arrange
            var dto = new UpdateQuoteDto
            {
                Id = 1,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<UpdateQuoteItemDto>
                {
                    new UpdateQuoteItemDto
                    {
                        Id = 1,
                        Description = "Updated Description",
                        Quantity = 5,
                        UnitPrice = 200,
                        Order = 1
                    }
                }
            };

            var existingQuote = new Quote
            {
                Id = 1,
                CompanyId = 1,
                CustomerId = 1,
                UserId = 1,
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        Id = 1,
                        Description = "Old Description",
                        Quantity = 1,
                        UnitPrice = 100,
                        Order = 1
                    }
                }
            };

            Quote updatedQuote = null!;

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existingQuote);

            _quoteRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => updatedQuote = q)
                .ReturnsAsync((Quote q) => q);

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.UpdateAsync(dto, companyId: 1);

            // Assert
            Assert.IsNotNull(updatedQuote);
            var item = updatedQuote.QuoteItems.First();
            Assert.AreEqual("Updated Description", item.Description);
            Assert.AreEqual(5, item.Quantity);
            Assert.AreEqual(200, item.UnitPrice);
        }

        [TestMethod]
        public async Task UpdateAsync_WhenAddingNewItem_ShouldAddItemToQuote()
        {
            // Arrange
            var dto = new UpdateQuoteDto
            {
                Id = 1,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<UpdateQuoteItemDto>
                {
                    new UpdateQuoteItemDto
                    {
                        Id = 0, // New item
                        Description = "New Item",
                        Quantity = 3,
                        UnitPrice = 150,
                        Order = 2
                    }
                }
            };

            var existingQuote = new Quote
            {
                Id = 1,
                CompanyId = 1,
                CustomerId = 1,
                UserId = 1,
                QuoteItems = new List<QuoteItem>()
            };

            Quote updatedQuote = null!;

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existingQuote);

            _quoteRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => updatedQuote = q)
                .ReturnsAsync((Quote q) => q);

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.UpdateAsync(dto, companyId: 1);

            // Assert
            Assert.IsNotNull(updatedQuote);
            Assert.AreEqual(1, updatedQuote.QuoteItems.Count);
            var newItem = updatedQuote.QuoteItems.First();
            Assert.AreEqual("New Item", newItem.Description);
            Assert.AreEqual(3, newItem.Quantity);
            Assert.AreEqual(150, newItem.UnitPrice);
        }

        [TestMethod]
        public async Task UpdateAsync_WithMultipleItems_ShouldRecalculateTotalPrice()
        {
            // Arrange
            var dto = new UpdateQuoteDto
            {
                Id = 1,
                PaymentMethod = PaymentMethod.Cash,
                Items = new List<UpdateQuoteItemDto>
                {
                    new UpdateQuoteItemDto { Id = 0, Description = "Item 1", Quantity = 2, UnitPrice = 100, Order = 1 },
                    new UpdateQuoteItemDto { Id = 0, Description = "Item 2", Quantity = 3, UnitPrice = 50, Order = 2 }
                }
            };

            var existingQuote = new Quote
            {
                Id = 1,
                CompanyId = 1,
                CustomerId = 1,
                UserId = 1,
                QuoteItems = new List<QuoteItem>()
            };

            Quote updatedQuote = null!;

            _quoteRepositoryMock
                .Setup(x => x.GetByIdAsync(1))
                .ReturnsAsync(existingQuote);

            _quoteRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => updatedQuote = q)
                .ReturnsAsync((Quote q) => q);

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.UpdateAsync(dto, companyId: 1);

            // Assert - TotalPrice should be (2*100) + (3*50) = 350
            Assert.IsNotNull(updatedQuote);
            Assert.AreEqual(350m, updatedQuote.TotalPrice);
        }

        #endregion

        #region DeleteAsync Tests

        [TestMethod]
        public async Task DeleteAsync_WhenQuoteExists_ShouldReturnTrue()
        {
            // Arrange
            var quote = new Quote { Id = 1, CompanyId = 1 };

            _quoteRepositoryMock
                .Setup(x => x.DeleteAsync(1, 1))
                .ReturnsAsync(quote);

            // Act
            var result = await _quoteService.DeleteAsync(1, companyId: 1);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task DeleteAsync_WhenQuoteDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            _quoteRepositoryMock
                .Setup(x => x.DeleteAsync(999, 1))
                .ReturnsAsync((Quote?)null);

            // Act
            var result = await _quoteService.DeleteAsync(999, companyId: 1);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task DeleteAsync_ShouldCallRepositoryWithCorrectCompanyId()
        {
            // Arrange
            var companyId = 42;
            var quoteId = 1;

            _quoteRepositoryMock
                .Setup(x => x.DeleteAsync(quoteId, companyId))
                .ReturnsAsync(new Quote { Id = quoteId, CompanyId = companyId });

            // Act
            await _quoteService.DeleteAsync(quoteId, companyId);

            // Assert
            _quoteRepositoryMock.Verify(
                x => x.DeleteAsync(quoteId, companyId),
                Times.Once
            );
        }

        #endregion

        #region GetPagedAsync Tests

        [TestMethod]
        public async Task GetPagedAsync_ShouldPassFiltersToRepository()
        {
            // Arrange
            var filter = new QuoteFilterDto
            {
                SearchTerm = "test",
                CreatedAtStart = new DateTime(2024, 1, 1),
                CreatedAtEnd = new DateTime(2024, 12, 31),
                SortBy = "CreatedAt",
                SortDescending = true,
                Page = 2,
                PageSize = 20
            };

            var companyId = 1;

            _quoteRepositoryMock
                .Setup(x => x.GetPagedAsync(
                    It.IsAny<string>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                ))
                .ReturnsAsync(new PagedResult<Quote>(0, new List<Quote>(), 1, 10));

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns(new QuoteResponseDto());

            // Act
            await _quoteService.GetPagedAsync(filter, companyId);

            // Assert
            _quoteRepositoryMock.Verify(
                x => x.GetPagedAsync(
                    "test",
                    filter.CreatedAtStart,
                    filter.CreatedAtEnd,
                    "CreatedAt",
                    true,
                    2,
                    20,
                    companyId
                ),
                Times.Once
            );
        }

        [TestMethod]
        public async Task GetPagedAsync_ShouldMapQuotesToResponseDtos()
        {
            // Arrange
            var filter = new QuoteFilterDto();
            var quotes = new List<Quote>
            {
                new Quote { Id = 1, CompanyId = 1 },
                new Quote { Id = 2, CompanyId = 1 }
            };

            _quoteRepositoryMock
                .Setup(x => x.GetPagedAsync(
                    It.IsAny<string>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                ))
                .ReturnsAsync(new PagedResult<Quote>(2, quotes, 1, 10));

            _mapperMock
                .Setup(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns((Quote q) => new QuoteResponseDto { Id = q.Id });

            // Act
            var result = await _quoteService.GetPagedAsync(filter, companyId: 1);

            // Assert
            Assert.AreEqual(2, result.TotalItems);
            Assert.AreEqual(2, result.Items.Count());
            _mapperMock.Verify(x => x.Map<QuoteResponseDto>(It.IsAny<Quote>()), Times.Exactly(2));
        }

        [TestMethod]
        public async Task GetPagedAsync_ShouldReturnCorrectPageInfo()
        {
            // Arrange
            var filter = new QuoteFilterDto { Page = 3, PageSize = 15 };
            var quotes = new List<Quote>();

            _quoteRepositoryMock
                .Setup(x => x.GetPagedAsync(
                    It.IsAny<string>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<string>(),
                    It.IsAny<bool>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()
                ))
                .ReturnsAsync(new PagedResult<Quote>(100, quotes, 3, 15));

            // Act
            var result = await _quoteService.GetPagedAsync(filter, companyId: 1);

            // Assert
            Assert.AreEqual(100, result.TotalItems);
            Assert.AreEqual(3, result.Page);
            Assert.AreEqual(15, result.PageSize);
        }

        #endregion
    }
}

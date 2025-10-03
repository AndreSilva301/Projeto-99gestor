using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Services;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Domain;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManiaDeLimpeza.Application.UnitTests.Tools;

namespace ManiaDeLimpeza.Application.UnitTests.Services
{
    [TestClass]
    public class QuoteServiceTests: TestsBase
    {
        private Mock<IQuoteRepository> _quoteRepositoryMock = null!;
        private Mock<ICustomerRepository> _clientRepositoryMock = null!;
        private QuoteService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            base.InitializeTestBase();

            _quoteRepositoryMock = new Mock<IQuoteRepository>();
            _clientRepositoryMock = new Mock<ICustomerRepository>();

            var mapper = GetRequiredService<IMapper>();

            _service = new QuoteService(_quoteRepositoryMock.Object, _clientRepositoryMock.Object, mapper);
        }


        [TestMethod]
        public async Task CreateAsync_WithValidClient_ShouldSucceed()
        {
            var quoteDto = new QuoteDto
            {
                ClientId = 1,
                LineItems = new List<LineItemDto>
                {
                    new() { Description = "Item A", Quantity = 2, UnitPrice = 10, Total = 18 }
                },
                PaymentMethod = PaymentMethod.Cash,
                CashDiscount = 5
            };

            var user = new User { Id = 2, Name = "Tester" };

            _clientRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(new Customer { Id = 1 });

            var result = await _service.CreateAsync(quoteDto, user);

            _quoteRepositoryMock.Verify(r => r.AddAsync(It.Is<Quote>(q =>
                q.TotalPrice == 13 &&
                q.UserId == user.Id &&
                q.CreatedAt != default
            )), Times.Once);

            Assert.AreEqual(13, result.TotalPrice);
            Assert.AreEqual(user.Id, result.UserId);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CreateAsync_WithInvalidClient_ShouldThrow()
        {
            var quoteDto = new QuoteDto
            {
                ClientId = 99,
                LineItems = new List<LineItemDto>
        {
            new() { Description = "Invalid", Quantity = 1, UnitPrice = 10, Total = 10 }
        }
            };

            var user = new User { Id = 1 };

            _clientRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

            await _service.CreateAsync(quoteDto, user);
        }

        
        [TestMethod]
        public async Task ArchiveAsync_WithInvalidId_ShouldReturnFalse()
        {
            _quoteRepositoryMock.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((Quote?)null);

            var result = await _service.ArchiveAsync(42);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldRecalculateTotal()
        {
            var quoteDto = new QuoteDto
            {
                Id = 1,
                LineItems = new List<LineItemDto> { new() { Description = "B", Quantity = 1, UnitPrice = 50, Total = 45 } },
                CashDiscount = 5
            };


            var existingQuote = new Quote
            {
                Id = 1,
                UserId = 1,
                QuoteItems = new List<QuoteItem>
                {
                    new() { Description = "Old Item", Quantity = 2, UnitPrice = 20, TotalValue = 40 }
                },
                CashDiscount = null,
                TotalPrice = 40,
                PaymentMethod = PaymentMethod.Cash
            };

            _quoteRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingQuote);

            var result = await _service.UpdateAsync(quoteDto);

            Assert.AreEqual(40, result.TotalPrice);
            _quoteRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Quote>()), Times.Once);
        }

        [TestMethod]
        public async Task UpdateAsync_ShouldNotOverrideCreatedFields()
        {
            // Arrange
            var originalCreatedAt = new DateTime(2024, 01, 01, 12, 00, 00);
            var originalCreatedByUserId = 7;

            var quoteDto = new QuoteDto
            {
                Id = 1,
                ClientId = 1,
                LineItems = new List<LineItemDto>
                {
                    new() { Description = "Updated Item", Quantity = 1, UnitPrice = 100, Total = 95 }
                },
                CashDiscount = 5,
                PaymentMethod = PaymentMethod.CreditCard,
            };

            var existingQuote = new Quote
            {
                Id = 1,
                CreatedAt = originalCreatedAt,
                UserId = originalCreatedByUserId,
                QuoteItems = new List<QuoteItem>
                {
                    new() { Description = "Old Item", Quantity = 2, UnitPrice = 20, TotalValue = 40 }
                },
                CashDiscount = null,
                TotalPrice = 40,
                PaymentMethod = PaymentMethod.Cash
            };

            _quoteRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(existingQuote);

            _quoteRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Quote>()))
                .Callback<Quote>(quote =>
                {
                    // Assert inside the callback to verify the fields are preserved
                    Assert.AreEqual(originalCreatedAt, quote.CreatedAt, "CreatedAt should not be changed.");
                    Assert.AreEqual(originalCreatedByUserId, quote.UserId, "CreatedByUserId should not be changed.");
                });

            // Act
            var result = await _service.UpdateAsync(quoteDto);

            // Assert
            Assert.AreEqual(90, result.TotalPrice); // 95 - 5
            _quoteRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Quote>()), Times.Once);
        }
    }
}

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
    public class QuoteServiceTests : TestsBase
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
    }
}

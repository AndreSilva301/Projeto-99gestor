using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Services;
using ManiaDeLimpeza.Application.UnitTests.Tools;
using ManiaDeLimpeza.Domain.Entities;
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
        private QuoteService _service;

        [TestInitialize]
        public void Setup()
        {
            _quoteRepositoryMock = new Mock<IQuoteRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _mapperMock = new Mock<IMapper>();

            _mapperMock
                .Setup(m => m.Map<QuoteResponseDto>(It.IsAny<Quote>()))
                .Returns((Quote q) => new QuoteResponseDto
                {
                    Id = q.Id,
                    TotalPrice = q.TotalPrice,
                    Items = q.QuoteItems?.Select(i => new QuoteItemResponseDto
                    {
                        Id = i.Id,
                        Description = i.Description,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.TotalPrice
                    }).ToList() ?? new List<QuoteItemResponseDto>()
                });

            _service = new QuoteService(
                _quoteRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CriarAsync_ComClienteInvalido_DeveLancarExcecao()
        {
            var quoteDto = new QuoteDto
            {
                CustomerId = 99,
                LineItems = new List<QuoteItemResponseDto>
                {
                    new() { Description = "Serviço Inválido", Quantity = 1, UnitPrice = 10, TotalPrice = 10 }
                }
            };

            var usuario = new User { Id = 1 };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

            await _service.CreateAsync(quoteDto, usuario);
        }

        [TestMethod]
        public async Task CriarAsync_ComDadosValidos_DeveRetornarOrcamentoCriado()
        {
            var dto = new CreateQuoteDto
            {
                CustomerId = 1,
                Items = new List<CreateQuoteItemDto>
                {
                    new() { Description = "Serviço 1", Quantity = 2, UnitPrice = 50 },
                    new() { Description = "Serviço 2", Quantity = 1, UnitPrice = 100 }
                }
            };

            var cliente = new Customer { Id = 1, Name = "Cliente Teste" };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cliente);

            _quoteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Quote>())).Returns(Task.CompletedTask);

            var quotes = new List<Quote>();
            _quoteRepositoryMock.Setup(r => r.Query()).Returns(quotes.AsQueryable());

            var resultado = await _service.CreateAsync(dto, 1);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(200m, resultado.TotalPrice);
            _quoteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Quote>()), Times.Once);
        }
        [TestMethod]
        public async Task CriarAsync_ComDescontoAVista_DeveAplicarCorretamente()
        {
            var dto = new CreateQuoteDto
            {
                CustomerId = 1,
                PaymentConditions = "à vista",
                CashDiscount = 10,
                Items = new List<CreateQuoteItemDto>
                {
                    new() { Description = "Serviço", Quantity = 1, UnitPrice = 100 }
                }
            };

            var cliente = new Customer { Id = 1, Name = "Cliente Teste" };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(cliente);
            _quoteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Quote>())).Returns(Task.CompletedTask);

            var resultado = await _service.CreateAsync(dto, 1);

            Assert.AreEqual(90m, resultado.TotalPrice);
        }

        [TestMethod]
        public async Task ObterPorIdAsync_ComIdInexistente_DeveRetornarNulo()
        {
            _quoteRepositoryMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Quote?)null);

            var resultado = await _service.GetByIdAsync(999);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task DeletarAsync_ComIdInexistente_DeveRetornarFalso()
        {
            _quoteRepositoryMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Quote?)null);

            var resultado = await _service.DeleteAsync(99);

            Assert.IsFalse(resultado);
        }

        [TestMethod]
        public async Task DeletarAsync_ComIdExistente_DeveRemoverComSucesso()
        {
            var quote = new Quote { Id = 1 };
            _quoteRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(quote);

            var resultado = await _service.DeleteAsync(1);

            _quoteRepositoryMock.Verify(r => r.DeleteAsync(quote), Times.Once);
            Assert.IsTrue(resultado);
        }

        [TestMethod]
        public async Task AtualizarAsync_ComOrcamentoExistente_DeveRecalcularValores()
        {
            // Arrange
            var quote = new Quote
            {
                Id = 1,
                TotalPrice = 100,
                QuoteItems = new List<QuoteItem>
                {
                    new() { Id = 1, Description = "Serviço 1", Quantity = 1, UnitPrice = 100, TotalPrice = 100 }
                }
            };

            var dto = new UpdateQuoteDto
            {
                Id = 1,
                Items = new List<UpdateQuoteItemDto>
                {
                    new() { Id = 1, Description = "Serviço 1", Quantity = 2, UnitPrice = 100 }
                }
            };

            _quoteRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(quote);
            _quoteRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote q) => q);

            // Act
            var resultado = await _service.UpdateAsync(1, dto);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(200m, resultado.TotalPrice, "O valor total deve ser recalculado corretamente.");
            Assert.AreEqual(2, resultado.Items.First().Quantity, "A quantidade do item deve ser atualizada.");
            _quoteRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Quote>()), Times.Once);
        }

        [TestMethod]
        public async Task ObterTodosAsync_ComFiltros_DeveChamarRepositorioComParametrosCorretos()
        {
            // Arrange
            var filtro = new QuoteFilterDto
            {
                ClientName = "João",
                CreatedAtStart = new DateTime(2024, 1, 1),
                CreatedAtEnd = new DateTime(2024, 12, 31),
                Page = 1,
                PageSize = 10
            };

            _quoteRepositoryMock
                .Setup(r => r.GetAllAsync(
                    It.IsAny<int?>(),
                    It.IsAny<int?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<DateTime?>(),
                    It.IsAny<int>(),
                    It.IsAny<int>()))
                .ReturnsAsync(new List<Quote>());

            // Act
            var resultado = await _service.GetAllAsync(
                null,                   
                null,                   
                filtro.CreatedAtStart,
                filtro.CreatedAtEnd,
                filtro.Page,
                filtro.PageSize);

            // Assert
            _quoteRepositoryMock.Verify(r => r.GetAllAsync(
                It.IsAny<int?>(),
                It.IsAny<int?>(),
                It.Is<DateTime?>(d => d == filtro.CreatedAtStart),
                It.Is<DateTime?>(d => d == filtro.CreatedAtEnd),
                It.Is<int>(p => p == filtro.Page),
                It.Is<int>(s => s == filtro.PageSize)
            ), Times.Once);
        }
    }
}
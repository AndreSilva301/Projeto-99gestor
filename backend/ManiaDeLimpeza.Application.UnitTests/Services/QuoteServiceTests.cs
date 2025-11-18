using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Services;
using ManiaDeLimpeza.Application.UnitTests.Tools;
using ManiaDeLimpeza.Domain;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Persistence;
using Microsoft.EntityFrameworkCore;
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
        private ApplicationDbContext _dbContext;

        [TestInitialize]
        public void Setup()
        {
            _quoteRepositoryMock = new Mock<IQuoteRepository>();
            _customerRepositoryMock = new Mock<ICustomerRepository>();
            _mapperMock = new Mock<IMapper>();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
                .Options;

            _dbContext = new ApplicationDbContext(options);

            _mapperMock.Setup(m => m.Map<Quote>(It.IsAny<CreateQuoteDto>()))
                .Returns((CreateQuoteDto dto) => new Quote
                {
                    CustomerId = dto.CustomerId,
                    UserId = dto.UserId,
                    PaymentMethod = dto.PaymentMethod,
                    CashDiscount = dto.CashDiscount,
                    PaymentConditions = dto.PaymentConditions,
                    QuoteItems = dto.Items.Select(i => new QuoteItem
                    {
                        Description = i.Description,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        TotalPrice = i.Quantity.Value * i.UnitPrice.Value
                    }).ToList()
                });

            _mapperMock.Setup(m => m.Map<QuoteResponseDto>(It.IsAny<Quote>()))
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

            _mapperMock.Setup(m =>
                m.Map(It.IsAny<UpdateQuoteItemDto>(), It.IsAny<QuoteItem>()))
                .Callback((UpdateQuoteItemDto src, QuoteItem dest) =>
                {
                    dest.Quantity = src.Quantity;
                    dest.UnitPrice = src.UnitPrice;
                    dest.TotalPrice = src.Quantity * src.UnitPrice;
                });
            
            _quoteRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q =>
                {
                    q.Id = 10;
                    _dbContext.Quotes.Add(q);
                    _dbContext.SaveChanges();
                })
                .Returns(Task.CompletedTask);

            _quoteRepositoryMock
                .Setup(r => r.GetByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    return _dbContext.Quotes
                        .Include(x => x.QuoteItems)
                        .FirstOrDefault(q => q.Id == id);
                });

            _quoteRepositoryMock
                .Setup(r => r.Query())
                .Returns(_dbContext.Quotes.AsQueryable());

            _service = new QuoteService(
                _quoteRepositoryMock.Object,
                _customerRepositoryMock.Object,
                _mapperMock.Object,
                _dbContext
            );
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public async Task CriarAsync_ComClienteInvalido_DeveLancarExcecao()
        {
            var dto = new CreateQuoteDto
            {
                CustomerId = 99,
                UserId = 1,
                TotalPrice = 10m,
                PaymentMethod = PaymentMethod.Cash,
                Items = new()
                {
                    new QuoteItemDto { Description = "Item", Quantity = 1, UnitPrice = 10 }
                }
            };

            _customerRepositoryMock
                .Setup(r => r.GetByIdAsync(dto.CustomerId))
                .ReturnsAsync((Customer?)null);

            await _service.CreateAsync(dto, 1, 1);
        }

        [TestMethod]
        public async Task CriarAsync_ComDadosValidos_DeveRetornarOrcamentoCriado()
        {
            var dto = new CreateQuoteDto
            {
                CustomerId = 1,
                UserId = 1,
                TotalPrice = 0,
                PaymentMethod = PaymentMethod.Cash,
                Items = new()
        {
            new QuoteItemDto { Description = "Serviço 1", Quantity = 2, UnitPrice = 50 },
            new QuoteItemDto { Description = "Serviço 2", Quantity = 1, UnitPrice = 100 }
        }
            };

            var customer = new Customer { Id = 1, CompanyId = 1 };

            _customerRepositoryMock
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(customer);

            _quoteRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .Callback<Quote>(q => _dbContext.Quotes.Add(q)) 
                .Returns(Task.CompletedTask);

            _quoteRepositoryMock
                .Setup(r => r.Query())
                .Returns(_dbContext.Quotes);

            var resultado = await _service.CreateAsync(dto, 1, 1);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(200m, resultado.TotalPrice);
            _quoteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Quote>()), Times.Once);
        }

        [TestMethod]
        public async Task CriarAsync_ComDescontoAVista_DeveAplicarDesconto()
        {
            var dto = new CreateQuoteDto
            {
                CustomerId = 1,
                UserId = 1,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "à vista",
                CashDiscount = 10m,
                Items = new()
                {
                    new QuoteItemDto { Description = "Serviço", Quantity = 1, UnitPrice = 100 }
                }
            };

            _customerRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(new Customer { Id = 1, CompanyId = 1 });

            _quoteRepositoryMock
                .Setup(r => r.AddAsync(It.IsAny<Quote>()))
                .Returns(Task.CompletedTask);

            var resultado = await _service.CreateAsync(dto, 1, 1);

            Assert.AreEqual(90m, resultado.TotalPrice);
        }

        [TestMethod]
        public async Task ObterPorIdAsync_ComIdInexistente_DeveRetornarNulo()
        {
            _quoteRepositoryMock.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Quote?)null);

            var resultado = await _service.GetByIdAsync(999, 1);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task DeletarAsync_ComIdExistente_DeveRemover()
        {
            var quote = new Quote { Id = 1, CompanyId = 1 };

            _quoteRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(quote);

            var ok = await _service.DeleteAsync(1, 1);

            Assert.IsTrue(ok);
            _quoteRepositoryMock.Verify(r => r.DeleteAsync(quote), Times.Once);
        }

        [TestMethod]
        public async Task AtualizarAsync_DeveRecalcularValores()
        {
            var quote = new Quote
            {
                Id = 1,
                CompanyId = 1,
                Customer = new Customer { Id = 1, CompanyId = 1 },
                QuoteItems = new()
        {
            new QuoteItem { Id = 1, Quantity = 1, UnitPrice = 100, TotalPrice = 100 }
        }
            };

            var dto = new UpdateQuoteDto
            {
                Id = 1,
                Items = new()
                {
                    new UpdateQuoteItemDto { Id = 1, Quantity = 2, UnitPrice = 100 }
                }
            };

            _quoteRepositoryMock.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(quote);

            _quoteRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<Quote>()))
                .ReturnsAsync((Quote q) => q);

            var resultado = await _service.UpdateAsync(1, dto, 1);

            Assert.AreEqual(200m, resultado.TotalPrice);
            Assert.AreEqual(2, resultado.Items.First().Quantity);
        }

        [TestMethod]
        public async Task GetAllAsync_ChamaRepositorioComFiltros()
        {
            _quoteRepositoryMock
                .Setup(r => r.Query())
                .Returns(_dbContext.Quotes);

            var result = await _service.GetAllAsync(
                companyId: 1,
                customerId: null,
                userId: null,
                startDate: new DateTime(2024, 1, 1),
                endDate: new DateTime(2024, 12, 31),
                pageNumber: 1,
                pageSize: 10);

            Assert.IsNotNull(result);
            _quoteRepositoryMock.Verify(r => r.Query(), Times.Once);
        }
    }
}

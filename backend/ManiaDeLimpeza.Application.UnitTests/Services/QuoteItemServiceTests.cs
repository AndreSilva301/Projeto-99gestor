using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Services;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using Moq;

namespace ManiaDeLimpeza.Application.UnitTests.Services
{
    [TestClass]
    public class QuoteItemServiceTests
    {
        private Mock<IQuoteItemRepository> _quoteItemRepoMock = null!;
        private Mock<IQuoteRepository> _quoteRepoMock = null!;
        private QuoteItemService _service = null!;
        private Quote _quote = null!;

        [TestInitialize]
        public void Setup()
        {
            _quoteItemRepoMock = new Mock<IQuoteItemRepository>();
            _quoteRepoMock = new Mock<IQuoteRepository>();
            _service = new QuoteItemService(_quoteItemRepoMock.Object, _quoteRepoMock.Object);

            _quote = new Quote
            {
                Id = 1,
                TotalPrice = 200,
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem { Id = 1, QuoteId = 1, Description = "Serviço 1", Quantity = 2, UnitPrice = 50, TotalPrice = 100, Order = 1 },
                    new QuoteItem { Id = 2, QuoteId = 1, Description = "Serviço 2", Quantity = 1, UnitPrice = 100, TotalPrice = 100, Order = 2 }
                }
            };
        }

        [TestMethod]
        public async Task AdicionarItem_Valido_DeveAdicionarERecalcularTotal()
        {
            var dto = new CreateQuoteItemDto { Description = "Novo Serviço", Quantity = 1, UnitPrice = 150 };
            _quoteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(_quote);
            _quoteItemRepoMock.Setup(r => r.GetByQuoteIdAsync(1)).ReturnsAsync(_quote.QuoteItems);
            _quoteItemRepoMock.Setup(r => r.CreateAsync(It.IsAny<QuoteItem>()))
                              .ReturnsAsync((QuoteItem qi) => { qi.Id = 3; return qi; });
            _quoteRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Quote>())).ReturnsAsync(It.IsAny<Quote>());

            var resultado = await _service.AddItemAsync(1, dto);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(3, resultado.Order);
            _quoteRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Quote>()), Times.Once);
        }

        [TestMethod]
        public async Task AtualizarItem_Existente_DeveRecalcularTotal()
        {
            var item = _quote.QuoteItems.First();
            var dto = new UpdateQuoteItemDto
            {
                Description = "Serviço Atualizado",
                Quantity = 3,
                UnitPrice = 50
            };

            _quoteItemRepoMock.Setup(r => r.GetByIdAsync(item.Id)).ReturnsAsync(item);
            _quoteItemRepoMock.Setup(r => r.UpdateAsync(It.IsAny<QuoteItem>()))
                              .ReturnsAsync((QuoteItem qi) => qi);
            _quoteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(_quote);
            _quoteItemRepoMock.Setup(r => r.GetByQuoteIdAsync(1)).ReturnsAsync(_quote.QuoteItems);
            _quoteRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Quote>())).ReturnsAsync(It.IsAny<Quote>());

            var resultado = await _service.UpdateItemAsync(item.Id, dto);

            Assert.AreEqual("Serviço Atualizado", resultado.Description);
            _quoteRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Quote>()), Times.Once);
        }

        [TestMethod]
        public async Task DeletarItem_Existente_DeveRemoverERecalcularTotal()
        {
            var item = _quote.QuoteItems.First();
            _quoteItemRepoMock.Setup(r => r.GetByIdAsync(item.Id)).ReturnsAsync(item);
            _quoteItemRepoMock.Setup(r => r.DeleteAsync(item.Id)).ReturnsAsync(true);
            _quoteRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(_quote);
            _quoteItemRepoMock.Setup(r => r.GetByQuoteIdAsync(1))
                              .ReturnsAsync(_quote.QuoteItems.Where(i => i.Id != item.Id));
            _quoteRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Quote>())).ReturnsAsync(It.IsAny<Quote>());

            var resultado = await _service.DeleteItemAsync(item.Id);

            Assert.IsTrue(resultado);
            _quoteRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Quote>()), Times.Once);
        }

        [TestMethod]
        public async Task ReordenarItens_OrdemValida_DeveAtualizarOrdem()
        {
            var ids = new List<int> { 2, 1 };
            _quoteItemRepoMock.Setup(r => r.ReorderAsync(1, ids)).ReturnsAsync(true);

            var resultado = await _service.ReorderItemsAsync(1, ids);

            Assert.IsTrue(resultado);
            _quoteItemRepoMock.Verify(r => r.ReorderAsync(1, ids), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task AdicionarItem_OrcamentoNaoExiste_DeveLancarExcecao()
        {
            _quoteRepoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Quote)null!);
            await _service.AddItemAsync(99, new CreateQuoteItemDto());
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task AtualizarItem_NaoExistente_DeveLancarExcecao()
        {
            _quoteItemRepoMock.Setup(r => r.GetByIdAsync(123)).ReturnsAsync((QuoteItem)null!);
            await _service.UpdateItemAsync(123, new UpdateQuoteItemDto());
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public async Task DeletarItem_NaoExistente_DeveLancarExcecao()
        {
            _quoteItemRepoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((QuoteItem)null!);
            await _service.DeleteItemAsync(5);
        }

        [TestMethod]
        public async Task DeletarItem_SemOrcamento_DeveExecutarSemErro()
        {
            var item = _quote.QuoteItems.First();
            _quoteItemRepoMock.Setup(r => r.GetByIdAsync(item.Id)).ReturnsAsync(item);
            _quoteItemRepoMock.Setup(r => r.DeleteAsync(item.Id)).ReturnsAsync(true);
            _quoteRepoMock.Setup(r => r.GetByIdAsync(item.QuoteId)).ReturnsAsync((Quote)null!);

            var resultado = await _service.DeleteItemAsync(item.Id);

            Assert.IsTrue(resultado);
            _quoteRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Quote>()), Times.Never);
        }
    }
}


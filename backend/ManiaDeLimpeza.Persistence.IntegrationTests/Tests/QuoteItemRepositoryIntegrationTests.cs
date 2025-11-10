using ManiaDeLimpeza.Domain;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Persistence.IntegrationTests.Repositories
{
    [TestClass]
    public class QuoteItemRepositoryIntegrationTests
    {
        private ApplicationDbContext _context;
        private QuoteRepository _quoteRepository;
        private QuoteItemRepository _quoteItemRepository;

        [TestInitialize]
        public void Inicializar()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _quoteRepository = new QuoteRepository(_context);
            _quoteItemRepository = new QuoteItemRepository(_context);
        }

        [TestCleanup]
        public void Limpar()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        private async Task<Quote> CriarOrcamentoAsync()
        {
            var quote = new Quote
            {
                CustomerId = 1,
                UserId = 1,
                CreatedAt = DateTime.UtcNow,
                TotalPrice = 200,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            };

            await _quoteRepository.CreateAsync(quote);
            return quote;
        }

        [TestMethod]
        public async Task CriarAsync_ItemValido_DeveRetornarItemCriado()
        {
            // Arrange
            var orcamento = await CriarOrcamentoAsync();

            var item = new QuoteItem
            {
                QuoteId = orcamento.Id,
                Description = "Serviço de limpeza",
                Quantity = 2,
                UnitPrice = 50,
                TotalPrice = 100,
                Order = 1
            };

            // Act
            var criado = await _quoteItemRepository.CreateAsync(item);

            // Assert
            Assert.IsNotNull(criado);
            Assert.IsTrue(criado.Id > 0);
            Assert.AreEqual("Serviço de limpeza", criado.Description);
        }

        [TestMethod]
        public async Task ObterPorIdAsync_IdExistente_DeveRetornarItem()
        {
            var orcamento = await CriarOrcamentoAsync();

            var item = new QuoteItem
            {
                QuoteId = orcamento.Id,
                Description = "Item de teste",
                Quantity = 1,
                UnitPrice = 80,
                TotalPrice = 80,
                Order = 1
            };

            await _quoteItemRepository.CreateAsync(item);

            // Act
            var resultado = await _quoteItemRepository.GetByIdAsync(item.Id);

            // Assert
            Assert.IsNotNull(resultado);
            Assert.AreEqual(item.Description, resultado.Description);
        }

        [TestMethod]
        public async Task ObterPorQuoteId_DeveRetornarItensOrdenados()
        {
            var orcamento = await CriarOrcamentoAsync();

            var itemA = new QuoteItem { QuoteId = orcamento.Id, Description = "Item B", Quantity = 1, UnitPrice = 50, TotalPrice = 50, Order = 2 };
            var itemB = new QuoteItem { QuoteId = orcamento.Id, Description = "Item A", Quantity = 1, UnitPrice = 100, TotalPrice = 100, Order = 1 };

            await _quoteItemRepository.CreateAsync(itemA);
            await _quoteItemRepository.CreateAsync(itemB);

            // Act
            var resultado = await _quoteItemRepository.GetByQuoteIdAsync(orcamento.Id);
            var lista = resultado.ToList();

            // Assert
            Assert.AreEqual(2, lista.Count);
            Assert.AreEqual("Item A", lista.First().Description);
        }

        [TestMethod]
        public async Task AtualizarAsync_ItemExistente_DeveAtualizarComSucesso()
        {
            var orcamento = await CriarOrcamentoAsync();
            var item = new QuoteItem
            {
                QuoteId = orcamento.Id,
                Description = "Item Original",
                Quantity = 1,
                UnitPrice = 50,
                TotalPrice = 50,
                Order = 1
            };

            await _quoteItemRepository.CreateAsync(item);

            item.Description = "Item Atualizado";
            item.UnitPrice = 60;
            item.TotalPrice = 60;

            // Act
            var atualizado = await _quoteItemRepository.UpdateAsync(item);

            // Assert
            Assert.AreEqual("Item Atualizado", atualizado.Description);
            Assert.AreEqual(60, atualizado.UnitPrice);
        }

        [TestMethod]
        public async Task DeletarAsync_ItemExistente_DeveRemoverComSucesso()
        {
            var orcamento = await CriarOrcamentoAsync();
            var item = new QuoteItem
            {
                QuoteId = orcamento.Id,
                Description = "Item para remover",
                Quantity = 1,
                UnitPrice = 70,
                TotalPrice = 70,
                Order = 1
            };
            await _quoteItemRepository.CreateAsync(item);

            // Act
            var sucesso = await _quoteItemRepository.DeleteAsync(item.Id);
            var removido = await _quoteItemRepository.GetByIdAsync(item.Id);

            // Assert
            Assert.IsTrue(sucesso);
            Assert.IsNull(removido);
        }

        [TestMethod]
        public async Task ReordenarAsync_DeveAtualizarOrdemDosItens()
        {
            var orcamento = await CriarOrcamentoAsync();
            var item1 = await _quoteItemRepository.CreateAsync(new QuoteItem { QuoteId = orcamento.Id, Description = "Item 1", Quantity = 1, UnitPrice = 10, TotalPrice = 10, Order = 1 });
            var item2 = await _quoteItemRepository.CreateAsync(new QuoteItem { QuoteId = orcamento.Id, Description = "Item 2", Quantity = 1, UnitPrice = 20, TotalPrice = 20, Order = 2 });

            var novaOrdem = new List<int> { item2.Id, item1.Id };

            // Act
            var sucesso = await _quoteItemRepository.ReorderAsync(orcamento.Id, novaOrdem);

            // Assert
            Assert.IsTrue(sucesso);

            var itensAtualizados = await _quoteItemRepository.GetByQuoteIdAsync(orcamento.Id);
            var lista = itensAtualizados.ToList();

            Assert.AreEqual(2, lista.Count);
            Assert.AreEqual(item2.Id, lista.First().Id);
        }

        [TestMethod]
        public async Task ObterPorIdAsync_IdInexistente_DeveRetornarNull()
        {
            var resultado = await _quoteItemRepository.GetByIdAsync(9999);
            Assert.IsNull(resultado);
        }

        [TestMethod]
        public async Task CriarAsync_QuoteInexistente_DeveLancarKeyNotFoundException()
        {
            var item = new QuoteItem
            {
                QuoteId = 9999,
                Description = "Item inválido",
                Quantity = 1,
                UnitPrice = 10,
                TotalPrice = 10,
                Order = 1
            };

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                async () => await _quoteItemRepository.CreateAsync(item)
            );
        }

        [TestMethod]
        public async Task GetByQuoteIdAsync_QuoteInexistente_DeveLancarKeyNotFoundException()
        {
            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                async () => await _quoteItemRepository.GetByQuoteIdAsync(9999)
            );
        }

        [TestMethod]
        public async Task UpdateAsync_ItemInexistente_DeveLancarKeyNotFoundException()
        {
            var item = new QuoteItem
            {
                Id = 9999,
                QuoteId = 1,
                Description = "Item inexistente",
                Quantity = 1,
                UnitPrice = 10,
                TotalPrice = 10,
                Order = 1
            };

            await Assert.ThrowsExceptionAsync<KeyNotFoundException>(
                async () => await _quoteItemRepository.UpdateAsync(item)
            );
        }

        [TestMethod]
        public async Task ReorderAsync_ItensFaltando_DeveLancarInvalidOperationException()
        {
            var orcamento = await CriarOrcamentoAsync();
            var item = await _quoteItemRepository.CreateAsync(new QuoteItem
            {
                QuoteId = orcamento.Id,
                Description = "Item único",
                Quantity = 1,
                UnitPrice = 10,
                TotalPrice = 10,
                Order = 1
            });

            var listaIncompleta = new List<int> { item.Id, 9999 };

            await Assert.ThrowsExceptionAsync<InvalidOperationException>(
                async () => await _quoteItemRepository.ReorderAsync(orcamento.Id, listaIncompleta)
            );
        }
    }
}

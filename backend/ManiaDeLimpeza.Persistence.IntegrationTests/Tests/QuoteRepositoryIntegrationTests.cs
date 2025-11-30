using ManiaDeLimpeza.Domain;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Persistence.IntegrationTests.Tools;
using ManiaDeLimpeza.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Persistence.IntegrationTests.Tests
{
    [TestClass]
    public class QuoteRepositoryIntegrationTests
    {
        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            TestDbContextFactory.InitializeConfiguration();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            using var db = TestDbContextFactory.CreateContext();
            await db.Database.ExecuteSqlRawAsync("DELETE FROM [QuoteItems]");
            await db.Database.ExecuteSqlRawAsync("DELETE FROM [Quotes]");
            await db.Database.ExecuteSqlRawAsync("DELETE FROM [Customers]");
            await db.Database.ExecuteSqlRawAsync("DELETE FROM [Users]");
            await db.Database.ExecuteSqlRawAsync("DELETE FROM [Companies]");
        }

        private async Task<Company> CriarEmpresaTesteAsync(ApplicationDbContext db)
        {
            var empresa = new Company
            {
                Name = "Empresa Teste",
                CNPJ = "12345678000199"
            };
            db.Companies.Add(empresa);
            await db.SaveChangesAsync();
            return empresa;
        }

        private async Task<User> CriarUsuarioTesteAsync(ApplicationDbContext db, int companyId)
        {
            var usuario = new User
            {
                Name = "Usuário Teste",
                Email = $"usuario{Guid.NewGuid()}@test.com",
                PasswordHash = "hashedpassword",
                CompanyId = companyId,
                Profile = UserProfile.Admin
            };
            db.Users.Add(usuario);
            await db.SaveChangesAsync();
            return usuario;
        }

        private async Task<Customer> CriarClienteTesteAsync(ApplicationDbContext db, int companyId)
        {
            var cliente = new Customer
            {
                Name = "Cliente Teste",
                Email = $"cliente{Guid.NewGuid()}@test.com",
                CompanyId = companyId,
                Phone = new Phone { Mobile = "11999999999" }
            };
            db.Customers.Add(cliente);
            await db.SaveChangesAsync();
            return cliente;
        }

        [TestMethod]
        public async Task CreateAsync_DeveCriarOrcamentoComItens()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente = await CriarClienteTesteAsync(db, empresa.Id);

            var orcamento = new Quote
            {
                CompanyId = empresa.Id,
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                TotalPrice = 150.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista",
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        Description = "Limpeza residencial",
                        Quantity = 1,
                        UnitPrice = 150.00m,
                        TotalPrice = 150.00m,
                        Order = 1
                    }
                }
            };

            var resultado = await repo.AddAsync(orcamento);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.Id > 0);
            Assert.AreEqual(1, resultado.QuoteItems.Count);
            Assert.AreEqual("Limpeza residencial", resultado.QuoteItems.First().Description);
        }

        [TestMethod]
        public async Task DeleteAsync_DeveRemoverOrcamento()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente = await CriarClienteTesteAsync(db, empresa.Id);

            var orcamento = new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                CompanyId = empresa.Id,
                TotalPrice = 100.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            };

            var criado = await repo.AddAsync(orcamento);

            var removido = await repo.DeleteAsync(criado.Id, empresa.Id);

            Assert.IsNotNull(removido);

            var buscado = await repo.GetByIdAsync(criado.Id);

            Assert.IsNull(buscado);
        }

        [TestMethod]
        public async Task DeleteAsync_ComIdInexistente_DeveRetornarFalse()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);

            var empresa = await CriarEmpresaTesteAsync(db);

            var sucesso = await repo.DeleteAsync(9999, empresa.Id);

            Assert.IsNull(sucesso);
        }

        [TestMethod]
        public async Task CountAsync_SemFiltros_DeveRetornarTotalDeOrcamentos()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente = await CriarClienteTesteAsync(db, empresa.Id);

            await repo.AddAsync(new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                CompanyId = empresa.Id,
                TotalPrice = 100.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            });

            await repo.AddAsync(new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                CompanyId = empresa.Id,
                TotalPrice = 200.00m,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "Pix"
            });

            var result = await repo.GetPagedAsync(
                searchTerm: null,
                createdAtStart: null,
                createdAtEnd: null,
                sortBy: "",
                sortDescending: false,
                page: 1,
                pageSize: 10,
                companyId: empresa.Id
            );

            Assert.AreEqual(2, result.TotalItems);
        }


        [TestMethod]
        public async Task CountAsync_FiltradoPorCliente_DeveRetornarContagemDoCliente()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente1 = await CriarClienteTesteAsync(db, empresa.Id);
            var cliente2 = await CriarClienteTesteAsync(db, empresa.Id);

            // Update customer names and save to database
            cliente1.Name = "Mariana";
            cliente2.Name = "Roberval";
            db.Customers.Update(cliente1);
            db.Customers.Update(cliente2);
            await db.SaveChangesAsync();

            await repo.AddAsync(new Quote
            {
                CustomerId = cliente1.Id,
                UserId = usuario.Id,
                CompanyId = empresa.Id,
                TotalPrice = 100.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            }); 

            await repo.AddAsync(new Quote
            {
                CustomerId = cliente2.Id,
                UserId = usuario.Id,
                CompanyId = empresa.Id,
                TotalPrice = 200.00m,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "Pix"
            }); 

            var resultado = await repo.GetPagedAsync(
                searchTerm: "Mariana",
                createdAtStart: null,
                createdAtEnd: null,
                sortBy: "",
                sortDescending: false,
                page: 1,
                pageSize: 50,
                companyId: empresa.Id
            );

            Assert.AreEqual(1, resultado.TotalItems);
        }

        [TestMethod]
        public async Task ExistsAsync_ComIdExistente_DeveRetornarTrue()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);

            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente = await CriarClienteTesteAsync(db, empresa.Id);

            var orcamento = new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                CompanyId = empresa.Id,
                TotalPrice = 100.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            };

            await repo.AddAsync(orcamento); 

            var existe = await repo.ExistsAsync(orcamento.Id, empresa.Id); 

            Assert.IsTrue(existe);
        }

        [TestMethod]
        public async Task ExistsAsync_ComIdInexistente_DeveRetornarFalse()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);

            var empresa = await CriarEmpresaTesteAsync(db);

            var existe = await repo.ExistsAsync(9999, empresa.Id); 

            Assert.IsFalse(existe);
        }

        [TestMethod]
        public async Task DeleteAsync_DeveCascatearParaQuoteItems()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente = await CriarClienteTesteAsync(db, empresa.Id);

            var orcamento = new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                CompanyId = empresa.Id,
                TotalPrice = 300.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista",
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        Description = "Item 1",
                        Quantity = 1,
                        UnitPrice = 150.00m,
                        TotalPrice = 150.00m,
                        Order = 1
                    },
                    new QuoteItem
                    {
                        Description = "Item 2",
                        Quantity = 1,
                        UnitPrice = 150.00m,
                        TotalPrice = 150.00m,
                        Order = 2
                    }
                }
            };

            await repo.AddAsync(orcamento);

            var itensAntes = await db.QuoteItems.Where(qi => qi.QuoteId == orcamento.Id).CountAsync();
            Assert.AreEqual(2, itensAntes);

            await repo.DeleteAsync(orcamento.Id, empresa.Id);

            var itensDepois = await db.QuoteItems.Where(qi => qi.QuoteId == orcamento.Id).CountAsync();
            Assert.AreEqual(0, itensDepois);
        }

        [TestMethod]
        public async Task CreateAsync_ComCashDiscount_DeveCalcularTotalCorretamente()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente = await CriarClienteTesteAsync(db, empresa.Id);

            var orcamento = new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                CompanyId = empresa.Id,
                TotalPrice = 100.00m,
                CashDiscount = 10.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista com desconto"
            };

            var resultado = await repo.AddAsync(orcamento);

            Assert.IsNotNull(resultado);
            Assert.AreEqual(10.00m, resultado.CashDiscount);
            Assert.AreEqual(100.00m, resultado.TotalPrice);
        }

        [TestMethod]
        public async Task CreateAsync_ComCustomFieldsEmQuoteItem_DevePersistirJson()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente = await CriarClienteTesteAsync(db, empresa.Id);

            var orcamento = new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                CompanyId = empresa.Id,
                TotalPrice = 150.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista",
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        Description = "Limpeza com extras",
                        Quantity = 1,
                        UnitPrice = 150.00m,
                        TotalPrice = 150.00m,
                        Order = 1,
                        CustomFields = new Dictionary<string, string>
                        {
                            { "Tipo", "Residencial" },
                            { "Metragem", "100m2" },
                            { "Observacao", "Cliente VIP" }
                        }
                    }
                }
            };

            var resultado = await repo.AddAsync(orcamento);

            // Buscar novamente para verificar persistência
            var buscado = await repo.GetByIdAsync(resultado.Id);

            Assert.IsNotNull(buscado);
            Assert.AreEqual(1, buscado.QuoteItems.Count);
            
            var item = buscado.QuoteItems.First();
            Assert.IsNotNull(item.CustomFields);
            Assert.AreEqual(3, item.CustomFields.Count);
            Assert.AreEqual("Residencial", item.CustomFields["Tipo"]);
            Assert.AreEqual("100m2", item.CustomFields["Metragem"]);
            Assert.AreEqual("Cliente VIP", item.CustomFields["Observacao"]);
        }
    }
}

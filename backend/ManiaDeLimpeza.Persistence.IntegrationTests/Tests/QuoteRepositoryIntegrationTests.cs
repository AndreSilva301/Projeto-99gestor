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

            var resultado = await repo.CreateAsync(orcamento, empresa.Id);

            Assert.IsNotNull(resultado);
            Assert.IsTrue(resultado.Id > 0);
            Assert.AreEqual(1, resultado.QuoteItems.Count);
            Assert.AreEqual("Limpeza residencial", resultado.QuoteItems.First().Description);
        }

        [TestMethod]
        public async Task GetByIdAsync_DeveRetornarOrcamentoComIncludes()
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
                TotalPrice = 250.00m,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "Pix após conclusão",
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        Description = "Item 1",
                        Quantity = 2,
                        UnitPrice = 100.00m,
                        TotalPrice = 200.00m,
                        Order = 1
                    },
                    new QuoteItem
                    {
                        Description = "Item 2",
                        Quantity = 1,
                        UnitPrice = 50.00m,
                        TotalPrice = 50.00m,
                        Order = 2
                    }
                }
            };

            var resultado = await repo.CreateAsync(orcamento, empresa.Id);

            var buscado = await repo.GetByIdAsync(orcamento.Id);

            Assert.IsNotNull(buscado);
            Assert.AreEqual(orcamento.Id, buscado.Id);
            Assert.IsNotNull(buscado.Customer);
            Assert.IsNotNull(buscado.User);
            Assert.AreEqual(2, buscado.QuoteItems.Count);
            Assert.AreEqual("Cliente Teste", buscado.Customer.Name);
            Assert.AreEqual("Usuário Teste", buscado.User.Name);
        }

        [TestMethod]
        public async Task UpdateAsync_DeveAtualizarOrcamento()
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
                TotalPrice = 100.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista",
                QuoteItems = new List<QuoteItem>
                {
                    new QuoteItem
                    {
                        Description = "Serviço original",
                        Quantity = 1,
                        UnitPrice = 100.00m,
                        TotalPrice = 100.00m,
                        Order = 1
                    }
                }
            };

            var resultado = await repo.CreateAsync(orcamento, empresa.Id);

            // Atualizar
            orcamento.TotalPrice = 200.00m;
            orcamento.PaymentMethod = PaymentMethod.CreditCard;
            orcamento.PaymentConditions = "Cartão 3x sem juros";
            orcamento.UpdatedAt = DateTime.UtcNow;

            var atualizado = await repo.UpdateAsync(orcamento, empresa.Id);

            Assert.AreEqual(200.00m, atualizado.TotalPrice);
            Assert.AreEqual(PaymentMethod.CreditCard, atualizado.PaymentMethod);
            Assert.AreEqual("Cartão 3x sem juros", atualizado.PaymentConditions);
            Assert.IsNotNull(atualizado.UpdatedAt);
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
                TotalPrice = 100.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            };

            var criado = await repo.CreateAsync(orcamento, empresa.Id);

            var removido = await repo.DeleteAsync(criado.Id, empresa.Id);

            Assert.IsTrue(removido);

            var buscado = await repo.GetByIdAsync(criado.Id, empresa.Id);

            Assert.IsNull(buscado);
        }

        [TestMethod]
        public async Task DeleteAsync_ComIdInexistente_DeveRetornarFalse()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);

            var empresa = await CriarEmpresaTesteAsync(db);

            var sucesso = await repo.DeleteAsync(9999, empresa.Id);

            Assert.IsFalse(sucesso);
        }

        [TestMethod]
        public async Task GetAllAsync_SemFiltros_DeveRetornarTodosOrcamentos()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente = await CriarClienteTesteAsync(db, empresa.Id);

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                TotalPrice = 100m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            }, empresa.Id);

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                TotalPrice = 200m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "Parcelado"
            }, empresa.Id);

            var resultado = await repo.GetAllAsync();

            Assert.AreEqual(2, resultado.Count());
        }

        [TestMethod]
        public async Task GetAllAsync_FiltradoPorCliente_DeveRetornarApenasDoCliente()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);

            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente1 = await CriarClienteTesteAsync(db, empresa.Id);
            var cliente2 = await CriarClienteTesteAsync(db, empresa.Id);

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente1.Id,
                UserId = usuario.Id,
                TotalPrice = 100.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            }, empresa.Id);

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente1.Id,
                UserId = usuario.Id,
                TotalPrice = 150.00m,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "Pix"
            }, empresa.Id);

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente2.Id,
                UserId = usuario.Id,
                TotalPrice = 200.00m,
                PaymentMethod = PaymentMethod.CreditCard,
                PaymentConditions = "Cartão"
            }, empresa.Id);

            var resultado = await repo.GetPagedAsync(
                searchTerm: null,
                createdAtStart: null,
                createdAtEnd: null,
                sortBy: "",
                sortDescending: false,
                page: 1,
                pageSize: 100,
                companyId: empresa.Id
            );

            var listaFiltrada = resultado.Items.Where(q => q.CustomerId == cliente1.Id).ToList();

            Assert.AreEqual(2, listaFiltrada.Count);
            Assert.IsTrue(listaFiltrada.All(q => q.CustomerId == cliente1.Id));
        }

        [TestMethod]
        public async Task GetAllAsync_FiltradoPorUsuario_DeveRetornarApenasDoUsuario()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario1 = await CriarUsuarioTesteAsync(db, empresa.Id);
            var usuario2 = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente = await CriarClienteTesteAsync(db, empresa.Id);

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario1.Id,
                TotalPrice = 100m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            }, empresa.Id);

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario2.Id,
                TotalPrice = 200m,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "Pix"
            }, empresa.Id);

            var resultado = await repo.GetPagedAsync(
                searchTerm: usuario1.Name,
                createdAtStart: null,
                createdAtEnd: null,
                sortBy: "",
                sortDescending: false,
                page: 1,
                pageSize: 20,
                companyId: empresa.Id
            );

            Assert.AreEqual(1, resultado.Items.Count);
            Assert.AreEqual(usuario1.Id, resultado.Items.First().UserId);
        }

        [TestMethod]
        public async Task GetAllAsync_FiltradoPorData_DeveRetornarApenasNoPeriodo()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente = await CriarClienteTesteAsync(db, empresa.Id);

            var dataInicio = new DateTime(2024, 1, 1);
            var dataFim = new DateTime(2024, 1, 31);

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                TotalPrice = 100.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista",
                CreatedAt = new DateTime(2024, 1, 15)
            }, empresa.Id);

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                TotalPrice = 200.00m,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "Pix",
                CreatedAt = new DateTime(2024, 2, 15)
            }, empresa.Id);

            var resultado = await repo.GetPagedAsync(
                searchTerm: null,
                createdAtStart: dataInicio,
                createdAtEnd: dataFim,
                sortBy: "",
                sortDescending: false,
                page: 1,
                pageSize: 50,
                companyId: empresa.Id
            );

            Assert.AreEqual(1, resultado.Items.Count);
            Assert.IsTrue(resultado.Items.First().CreatedAt >= dataInicio);
            Assert.IsTrue(resultado.Items.First().CreatedAt <= dataFim);
        }

        [TestMethod]
        public async Task GetAllAsync_ComPaginacao_DeveRetornarPaginaCorreta()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);

            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente = await CriarClienteTesteAsync(db, empresa.Id);

            for (int i = 1; i <= 5; i++)
            {
                await repo.CreateAsync(new Quote
                {
                    CustomerId = cliente.Id,
                    UserId = usuario.Id,
                    TotalPrice = i * 100.00m,
                    PaymentMethod = PaymentMethod.Cash,
                    PaymentConditions = "À vista",
                    CreatedAt = DateTime.UtcNow.AddDays(-i)
                }, empresa.Id);
            }

            var pagina1 = await repo.GetPagedAsync(
                searchTerm: null,
                createdAtStart: null,
                createdAtEnd: null,
                sortBy: "CreatedAt",
                sortDescending: true,
                page: 1,
                pageSize: 2,
                companyId: empresa.Id
            );

            var pagina2 = await repo.GetPagedAsync(
                searchTerm: null,
                createdAtStart: null,
                createdAtEnd: null,
                sortBy: "CreatedAt",
                sortDescending: true,
                page: 2,
                pageSize: 2,
                companyId: empresa.Id
            );

            Assert.AreEqual(2, pagina1.Items.Count);
            Assert.AreEqual(2, pagina2.Items.Count);

            var listaPagina1 = pagina1.Items.ToList();
            Assert.IsTrue(listaPagina1[0].CreatedAt >= listaPagina1[1].CreatedAt);
        }

        [TestMethod]
        public async Task CountAsync_SemFiltros_DeveRetornarTotalDeOrcamentos()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new QuoteRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);
            var usuario = await CriarUsuarioTesteAsync(db, empresa.Id);
            var cliente = await CriarClienteTesteAsync(db, empresa.Id);

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                TotalPrice = 100.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            }, empresa.Id);

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente.Id,
                UserId = usuario.Id,
                TotalPrice = 200.00m,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "Pix"
            }, empresa.Id);

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

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente1.Id,
                UserId = usuario.Id,
                TotalPrice = 100.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            }, empresa.Id); 

            await repo.CreateAsync(new Quote
            {
                CustomerId = cliente2.Id,
                UserId = usuario.Id,
                TotalPrice = 200.00m,
                PaymentMethod = PaymentMethod.Pix,
                PaymentConditions = "Pix"
            }, empresa.Id); 

            var resultado = await repo.GetPagedAsync(
                searchTerm: null,
                createdAtStart: null,
                createdAtEnd: null,
                sortBy: "",
                sortDescending: false,
                page: 1,
                pageSize: 50,
                companyId: empresa.Id
            );

            var totalCliente1 = resultado.Items.Count(q => q.CustomerId == cliente1.Id);

            Assert.AreEqual(1, totalCliente1);
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
                TotalPrice = 100.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista"
            };

            await repo.CreateAsync(orcamento, empresa.Id); 

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

            await repo.CreateAsync(orcamento, empresa.Id);

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
                TotalPrice = 100.00m,
                CashDiscount = 10.00m,
                PaymentMethod = PaymentMethod.Cash,
                PaymentConditions = "À vista com desconto"
            };

            var resultado = await repo.CreateAsync(orcamento, empresa.Id);

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

            var resultado = await repo.CreateAsync(orcamento, empresa.Id);

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

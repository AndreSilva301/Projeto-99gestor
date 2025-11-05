using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Persistence.IntegrationTests.Tools;
using ManiaDeLimpeza.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Persistence.IntegrationTests.Tests
{
    [TestClass]
    public class CustomerRepositoryIntegrationTests
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
            await db.Database.ExecuteSqlRawAsync("DELETE FROM [CustomerRelationships]");
            await db.Database.ExecuteSqlRawAsync("DELETE FROM [Customers]");
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

        [TestMethod]
        public async Task AdicionarCustomer_DeveSalvarCorretamente()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new CustomerRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);

            var cliente = new Customer
            {
                Name = "Cliente Joana",
                Email = "joana@test.com",
                CompanyId = empresa.Id,
                Phone = new Phone { Mobile = "11999999999" }
            };

            await repo.AddAsync(cliente);
            var todos = await repo.GetAllAsync();

            Assert.AreEqual(1, todos.Count());
            Assert.AreEqual("Cliente Joana", todos.First().Name);
        }

        [TestMethod]
        public async Task CRUD_CustomerRelationship_DeveInserirAtualizarRemover()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new CustomerRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);

            var cliente = new Customer
            {
                Name = "Cliente Relacionamento",
                Email = "rela@test.com",
                CompanyId = empresa.Id,
                Phone = new Phone { Mobile = "11999999999" }
            };
            await repo.AddAsync(cliente);

            // Adicionar relacionamento
            var rel = new CustomerRelationship
            {
                CustomerId = cliente.Id,
                Description = "Primeiro relacionamento"
            };
            var resultAdd = await repo.AddOrUpdateRelationshipsAsync(cliente.Id, new[] { rel });
            Assert.AreEqual(1, resultAdd.Count());
            Assert.IsTrue(resultAdd.First().Id > 0);

            // Atualizar relacionamento
            var relAtualiza = resultAdd.First();
            relAtualiza.Description = "Rel atualizado";
            var resultUpdate = await repo.AddOrUpdateRelationshipsAsync(cliente.Id, new[] { relAtualiza });
            Assert.AreEqual("Rel atualizado", resultUpdate.First().Description);

            // Remover relacionamento
            await repo.DeleteRelationshipsAsync(new[] { relAtualiza.Id }, cliente.Id);
            var relsRestantes = await repo.GetRelationshipsByCustomerAsync(cliente.Id);
            Assert.AreEqual(0, relsRestantes.Count());
        }

        [TestMethod]
        public async Task ConsultaPorIdComRelationships_DeveCarregarIncluindoRelacionamentos()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new CustomerRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);

            var cliente = new Customer
            {
                Name = "Consulta Inclui Rel",
                Email = "inclui@test.com",
                CompanyId = empresa.Id,
                Phone = new Phone { Mobile = "11999999999" }
            };
            await repo.AddAsync(cliente);

            var rel1 = new CustomerRelationship
            {
                CustomerId = cliente.Id,
                Description = "Relacionamento A"
            };
            var rel2 = new CustomerRelationship
            {
                CustomerId = cliente.Id,
                Description = "Relacionamento B"
            };
            await repo.AddOrUpdateRelationshipsAsync(cliente.Id, new[] { rel1, rel2 });

            var buscado = await repo.GetbyIdWithRelationshipAsync(cliente.Id);

            Assert.IsNotNull(buscado);
            Assert.IsTrue(buscado.CostumerRelationships.Count >= 2);
        }

        [TestMethod]
        public async Task ConsultaPaginadaPorCompany_DeveTrazerSomenteDaEmpresa()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new CustomerRepository(db);
            var empresaA = await CriarEmpresaTesteAsync(db);
            var empresaB = await CriarEmpresaTesteAsync(db);

            await repo.AddAsync(new Customer { Name = "EmpresaA 1", CompanyId = empresaA.Id, Email = "a1@a.com", Phone = new Phone { Mobile = "119" } });
            await repo.AddAsync(new Customer { Name = "EmpresaA 2", CompanyId = empresaA.Id, Email = "a2@a.com", Phone = new Phone { Mobile = "219" } });
            await repo.AddAsync(new Customer { Name = "EmpresaB 1", CompanyId = empresaB.Id, Email = "b1@b.com", Phone = new Phone { Mobile = "319" } });

            var paginaA = await repo.GetPagedByCompanyAsync(empresaA.Id, page: 1, pageSize: 10, searchTerm: null, orderBy: "Name", direction: "Desc");
            Assert.AreEqual(2, paginaA.Items.Count);
            Assert.IsTrue(paginaA.Items.All(c => c.CompanyId == empresaA.Id));
        }

        [TestMethod]
        public async Task SoftDelete_DeveOcultarCustomerDeConsultas()
        {
            using var db = TestDbContextFactory.CreateContext();
            var repo = new CustomerRepository(db);
            var empresa = await CriarEmpresaTesteAsync(db);

            var cliente = new Customer
            {
                Name = "Invisível",
                CompanyId = empresa.Id,
                Email = "invi@test.com",
                Phone = new Phone { Mobile = "11999999999" }
            };
            await repo.AddAsync(cliente);

            await repo.SoftDeleteAsync(cliente.Id);

            var ativos = await repo.GetPagedByCompanyAsync(empresa.Id, page: 1, pageSize: 10, searchTerm: null, orderBy: "Name", direction: "asc");
            Assert.IsFalse(ativos.Items.Any(c => c.Id == cliente.Id));
        }
    }
}
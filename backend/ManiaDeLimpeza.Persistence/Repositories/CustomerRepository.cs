using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Persistence.Repositories
{
   
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository, IScopedDependency
    {
        protected readonly ApplicationDbContext _context;

        public CustomerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Customer?> GetbyIdWithRelationshipAsync(int id)
        {
            return await _context.Customers
                 .AsNoTracking()
                 .Include(c => c.CustomerRelationships)
                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<PagedResult<Customer>> GetPagedByCompanyAsync(int companyId, int page, int pageSize, string? searchTerm, string orderBy, string direction)
        {
            var query = _context.Customers
                .AsNoTracking()
                .Where(c => c.CompanyId == companyId && !c.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(c =>
                    EF.Functions.Collate(c.Name, "SQL_Latin1_General_CP1_CI_AI")
                        .Contains(EF.Functions.Collate(searchTerm, "SQL_Latin1_General_CP1_CI_AI"))
                    || EF.Functions.Collate(c.Email, "SQL_Latin1_General_CP1_CI_AI")
                        .Contains(EF.Functions.Collate(searchTerm, "SQL_Latin1_General_CP1_CI_AI"))
                );
            }

            var validColumns = new[] { "Name", "Email", "CreatedDate", "UpdatedDate" };
            orderBy = validColumns.Contains(orderBy) ? orderBy : "Name";

            query = direction.ToLower() == "desc"
                ? query.OrderByDescending(c => EF.Property<object>(c, orderBy))
                : query.OrderBy(c => EF.Property<object>(c, orderBy));

            int totalItems = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Customer>
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = items
            };
        }

        public async Task<IEnumerable<CustomerRelationship>> GetRelationshipsByCustomerAsync(int customerId)
        {
            return await _context.CustomerRelationships
                .AsNoTracking()
                .Where(r => r.CustomerId == customerId)
                .OrderByDescending(r => r.DateTime)
                .ToListAsync();
        }

        public async Task SoftDeleteAsync(int customerId)
        {
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer != null && !customer.IsDeleted)
            {
                customer.IsDeleted = true;
                customer.UpdatedDate = DateTime.UtcNow;
                _context.Customers.Update(customer);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CustomerRelationship>> AddOrUpdateRelationshipsAsync(int customerId, IEnumerable<CustomerRelationship> relationships)
        {
            var results = new List<CustomerRelationship>();
            foreach (var rel in relationships)
            {
                rel.CustomerId = customerId;
                if (rel.Id > 0)
                {
                    var existing = await _context.CustomerRelationships.FirstOrDefaultAsync(r => r.Id == rel.Id && r.CustomerId == customerId);
                    if (existing != null)
                    {
                        existing.Description = rel.Description;
                        existing.UpdatedDate = DateTime.UtcNow;
                        _context.CustomerRelationships.Update(existing);
                        results.Add(existing);
                    }
                }
                else
                {
                    rel.CreatedDate = DateTime.UtcNow;
                    _context.CustomerRelationships.Add(rel);
                    results.Add(rel);
                }
            }
            await _context.SaveChangesAsync();
            return results;
        }

        public async Task DeleteRelationshipsAsync(IEnumerable<int> relationshipIds, int customerId)
        {
            var relationships = await _context.CustomerRelationships
                .Where(r => relationshipIds.Contains(r.Id) && r.CustomerId == customerId)
                .ToListAsync();

            _context.CustomerRelationships.RemoveRange(relationships);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Customer>> SearchAsync(string searchTerm)
        {
            return await BuildScoredQuery(searchTerm).ToListAsync();
        }

        public async Task<PagedResult<Customer>> SearchPagedAsync(string searchTerm, int page, int pageSize, int companyId, string? orderBy = "Name", string direction = "asc")
        {
            var scoredQuery = BuildScoredQuery(searchTerm);

            var totalItems = await scoredQuery.CountAsync();

            var items = await scoredQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Customer>
            {
                TotalItems = totalItems,
                Page = page,
                PageSize = pageSize,
                Items = items
            };
        }

        private IQueryable<Customer> BuildScoredQuery(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return _context.Customers.AsQueryable();

            var words = searchTerm
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(w => w.ToLower())
                .ToArray();

            return _context.Customers
                .Select(client => new
                {
                    Client = client,
                    MatchScore = words.Count(word =>
                        (client.Name != null && client.Name.ToLower().Contains(word)) ||
                        (client.Phone.Mobile != null && client.Phone.Mobile.ToLower().Contains(word)) ||
                        (client.Phone.Landline != null && client.Phone.Landline.ToLower().Contains(word)))
                })
                .Where(x => x.MatchScore > 0)
                .OrderByDescending(x => x.MatchScore)
                .Select(x => x.Client)
                .AsQueryable();
        }

        private static string RemoveAccents(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var normalizedString = text.Normalize(System.Text.NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(System.Text.NormalizationForm.FormC);
        }
    }
}

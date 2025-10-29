using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Persistence.Repositories;
public class CustomerRelationshipRepository : ICustomerRelationshipRepository, IScopedDependency
{
    private readonly ApplicationDbContext _context;

    public CustomerRelationshipRepository(ApplicationDbContext context) 
    {
        _context = context;
    }

    public async Task<CustomerRelationship?> GetByIdAsync(int id)
    {
        return await _context.CustomerRelationships
            .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);
    }

    public async Task<IEnumerable<CustomerRelationship>> ListByCustomerIdAsync(int customerId)
    {
        return await _context.CustomerRelationships
            .Where(r => r.CustomerId == customerId && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task AddAsync(CustomerRelationship entity)
    {
        await _context.CustomerRelationships.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CustomerRelationship entity)
    {
        _context.CustomerRelationships.Update(entity);
        await _context.SaveChangesAsync();
    }
    public async Task<IEnumerable<CustomerRelationship>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.CustomerRelationships
            .Where(r => r.CustomerId == customerId && !r.IsDeleted)
            .ToListAsync();
    }

    public async Task DeleteRelationshipsAsync(IEnumerable<int> relationshipIds)
    {
        var entities = await _context.CustomerRelationships
            .Where(r => relationshipIds.Contains(r.Id))
            .ToListAsync();

        foreach (var entity in entities)
            entity.IsDeleted = true;

        await _context.SaveChangesAsync();
    }
}

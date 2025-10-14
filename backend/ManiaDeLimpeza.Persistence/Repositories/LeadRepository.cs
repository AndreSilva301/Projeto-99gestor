using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Application.Interfaces;

namespace ManiaDeLimpeza.Persistence.Repositories;
public class LeadRepository : ILeadRepository
{
    private readonly ApplicationDbContext _context;

    public LeadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Lead lead)
    {
        await _context.Leads.AddAsync(lead);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}
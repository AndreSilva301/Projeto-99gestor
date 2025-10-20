using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;

namespace ManiaDeLimpeza.Persistence.Repositories;
public class LeadRepository : BaseRepository<Lead>, ILeadRepository, IScopedDependency
{
    protected readonly ApplicationDbContext _context;
    public LeadRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }
}
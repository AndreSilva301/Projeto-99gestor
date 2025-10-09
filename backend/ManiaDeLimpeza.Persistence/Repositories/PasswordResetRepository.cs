using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Persistence.Repositories;
public class PasswordResetRepository : IPasswordResetRepository, IScopedDependency
{
    private readonly ApplicationDbContext _context;

    public PasswordResetRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PasswordResetToken token)
    {
        await _context.Set<PasswordResetToken>().AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task<PasswordResetToken?> GetByTokenAsync(string token)
    {
        return await _context.Set<PasswordResetToken>()
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Token == token);
    }
}

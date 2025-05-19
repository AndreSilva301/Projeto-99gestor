using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Persistence.Repositories
{
    public class ClientRepository : BaseRepository<Client>, IClientRepository, IScopedDependency
    {
        protected readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Client>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Client>();

            var words = searchTerm
                .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(w => w.ToLower())
                .ToArray();

            var query = _context.Clients.AsQueryable();

            return await query
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
                .ToListAsync();
        }
    }
}

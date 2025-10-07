using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Persistence;
using ManiaDeLimpeza.Persistence.Repositories; 
using Microsoft.EntityFrameworkCore;

namespace ManiaDeLimpeza.Infrastructure.Repositories
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Company?> GetByCnpjAsync(string cnpj)
        {
            return await _context.Companies.FirstOrDefaultAsync(c => c.CNPJ == cnpj);
        }
        public async Task<Company> CreateAsync(Company company)
        {
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
            return company;
        }
   
    }
}
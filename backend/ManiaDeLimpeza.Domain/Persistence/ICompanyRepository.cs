using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Domain.Persistence;
public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<Company> CreateAsync(Company company);
    Task<Company?> GetByCnpjAsync(string cnpj);
    Task<Company?> GetByNameAsync(string name);
    Task<Company?> UpdateAsync (Company company);
}



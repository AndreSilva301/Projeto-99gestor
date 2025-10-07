using ManiaDeLimpeza.Domain.Entities;

namespace ManiaDeLimpeza.Domain.Persistence;
public interface ICompanyRepository : IBaseRepository<Company>
{
    Task<Company> CreateAsync(Company company);
    Task<Company?> GetByCnpjAsync(string cnpj);
}



using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Interfaces;
public interface ICompanyServices
{
    Task<Company> CreateCompanyAsync(Company company);
    Task<Company> UpdateCompanyAsync(Company company);
    Task<Company> DeleteCompanyAsync(int companyId);
    Task<Company> GetByCnpjAsync (string cnpj);
}

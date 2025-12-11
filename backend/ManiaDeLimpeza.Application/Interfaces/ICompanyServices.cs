using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.AspNetCore.Http;
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
    Task<Company?> GetByIdAsync(int id, User currentUser);
    Task<IEnumerable<Company>> GetAllAsync(User currentUser);
    Task<Company?> UpdateCompanyAsync(int id, UpdateCompanyDto dto, User currentUser);
    Task<string?> GetLogoAsync(int companyId);
    Task<string?> UpdateLogoAsync(int companyId, IFormFile file, User currentUser);
}

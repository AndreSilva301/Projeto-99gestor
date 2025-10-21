using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;

namespace ManiaDeLimpeza.Application.Services;
public class CompanyServices : ICompanyServices, IScopedDependency
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IUserRepository _userRepository;
    public CompanyServices(ICompanyRepository companyRepository, IUserRepository userRepository)
    {
        _companyRepository = companyRepository;
        _userRepository = userRepository;
    }
    public async Task<Company> GetByCnpjAsync(string cnpj)
    {
        var company = await _companyRepository.GetByCnpjAsync(cnpj);
        if (company == null)
        {
            throw new Exception("Empresa não encontrada.");
        }
        return company;
    }

    public async Task<Company> CreateCompanyAsync(Company company)
    {
        Company existingCompany = null;

        if (!string.IsNullOrWhiteSpace(company.CNPJ))
        {
            existingCompany = await _companyRepository.GetByCnpjAsync(company.CNPJ);
            if (existingCompany != null)
            {
                return existingCompany;
            }
        }

        existingCompany = await _companyRepository.GetByNameAsync(company.Name);
        if (existingCompany != null)
        {
            return existingCompany;
        }

        var createdCompany = await _companyRepository.CreateAsync(company);
        return createdCompany;
    }

    public async Task<Company> UpdateCompanyAsync(Company company)
    {
        var existingCompany = await _companyRepository.GetByIdAsync(company.Id);
        if (existingCompany == null)
        {
            throw new Exception("Company not found");
        }
        // Atualiza os campos da empresa existente
        existingCompany.Name = company.Name;
        existingCompany.CNPJ = company.CNPJ;
        // Adicione outros campos conforme necessário
        await _companyRepository.UpdateAsync(existingCompany);
        return existingCompany;
    }
    public async Task<Company> DeleteCompanyAsync(int companyId)
    {
        var existingCompany = await _companyRepository.GetByIdAsync(companyId);
        if (existingCompany == null)
        {
            throw new Exception("Company not found.");
        }
        await _companyRepository.DeleteAsync(existingCompany);
        return existingCompany;
    }
    public async Task<IEnumerable<Company>> GetAllAsync(User currentUser)
    {
        try
        {
            if (currentUser.Profile == UserProfile.SystemAdmin)
                return await _companyRepository.GetAllAsync();

            var company = await _companyRepository.GetByIdAsync(currentUser.CompanyId);
            return company is not null ? new[] { company } : Enumerable.Empty<Company>();
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Erro ao buscar empresas.", ex);
        }
    }

    public async Task<Company?> GetByIdAsync(int id, User currentUser)
    {
        try
        {
            if (currentUser.Profile != UserProfile.SystemAdmin && currentUser.CompanyId != id)
                throw new UnauthorizedAccessException("Acesso não autorizado.");

            var company = await _companyRepository.GetByIdAsync(id);

            if (company is null)
                throw new KeyNotFoundException($"Empresa com ID {id} não encontrada.");

            return company;
        }
        catch (Exception ex) when (ex is not UnauthorizedAccessException and not KeyNotFoundException)
        {
            throw new ApplicationException($"Erro ao buscar empresa com ID {id}.", ex);
        }
    }
}
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;

namespace ManiaDeLimpeza.Application.Services;
public class CompanyServices : ICompanyServices
{
    private readonly ICompanyRepository _companyRepository;
    public CompanyServices(ICompanyRepository companyRepository)
    {
        _companyRepository = companyRepository;
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
        var existingCompany = await _companyRepository.GetByCnpjAsync(company.CNPJ); // vai verificar se a empresa já existe pelo CNPJ

        Company associatedCompany;

        if (existingCompany != null)
        {
            associatedCompany = existingCompany;
            return existingCompany;
        }
        else
        {
            var createdCompany = await _companyRepository.CreateAsync(company);
            return createdCompany;
        }
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
}

using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Dtos.Mappers;
public static class CompanyMapper
{
    public static CompanyDto ToDto(this Company company)
    {
        return new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            CNPJ = company.CNPJ,
            DateTime = company.DateTime,
            Address = new AddressDto
            {
                Street = company.Address.Street,
                Number = company.Address.Number,
                Complement = company.Address.Complement,
                Neighborhood = company.Address.Neighborhood,
                City = company.Address.City,
                State = company.Address.State,
                ZipCode = company.Address.ZipCode
            },
            Phone = new PhoneDto
            {
                Mobile = company.Phone.Mobile,
                Landline = company.Phone.Landline
            }
        };
    }

    public static void UpdateFromDto(this Company company, UpdateCompanyDto dto)
    {
        company.Name = dto.Name;
        company.CNPJ = dto.CNPJ;

        company.Address.Street = dto.Address.Street;
        company.Address.Number = dto.Address.Number;
        company.Address.Complement = dto.Address.Complement;
        company.Address.Neighborhood = dto.Address.Neighborhood;
        company.Address.City = dto.Address.City;
        company.Address.State = dto.Address.State;
        company.Address.ZipCode = dto.Address.ZipCode;

        company.Phone.Mobile = dto.Phone.Mobile;
        company.Phone.Landline = dto.Phone.Landline;
    }
}


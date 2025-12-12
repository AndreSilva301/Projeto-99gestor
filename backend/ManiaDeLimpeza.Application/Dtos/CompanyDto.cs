namespace ManiaDeLimpeza.Application.Dtos;
public class CompanyDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string? CNPJ { get; set; }

    public AddressDto Address { get; set; } = new AddressDto();

    public PhoneDto Phone { get; set; } = new PhoneDto();

    public DateTime DateTime { get; set; }

}

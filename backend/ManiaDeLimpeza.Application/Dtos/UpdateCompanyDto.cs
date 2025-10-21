using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class UpdateCompanyDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? CNPJ { get; set; }

    [Required]
    public AddressDto Address { get; set; } = new AddressDto();

    [Required]
    public PhoneDto Phone { get; set; } = new PhoneDto();
}

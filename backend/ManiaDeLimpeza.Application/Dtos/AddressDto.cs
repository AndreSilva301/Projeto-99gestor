using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class AddressDto
{
    [Required]
    public string Street { get; set; } = string.Empty;

    public string? Number { get; set; }

    public string? Complement { get; set; }

    public string? Neighborhood { get; set; }
    [Required]
    public string City { get; set; } = string.Empty;
    [Required]
    public string State { get; set; } = string.Empty;
    [Required]
    public string ZipCode { get; set; } = string.Empty;
}

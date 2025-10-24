using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class UpdateUserDto
{
    [Required(ErrorMessage = "O Nome é Obrigatório.")]
    public string Name { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "O e-mail informado é inválido.")]
    public string? Email { get; set; }

}

using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ManiaDeLimpeza.Application.Dtos;
public class UpdateUserDto : IBasicDto
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public UserProfile? Profile { get; set; }
    public bool IsValid()
    {
        return Validate().Count == 0;
    }

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("O nome é obrigatório.");

        if (!string.IsNullOrWhiteSpace(Email))
        {
            if (!Email.Contains("@") || !Email.Contains("."))
                errors.Add("O e-mail informado é inválido.");
        }

        if (Profile.HasValue && !Enum.IsDefined(typeof(UserProfile), Profile.Value))
            errors.Add("O perfil informado é inválido.");

        return errors;
    }
}


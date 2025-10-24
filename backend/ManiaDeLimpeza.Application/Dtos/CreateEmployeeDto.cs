using ManiaDeLimpeza.Application.Common;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Interfaces;
using System.Globalization;

namespace ManiaDeLimpeza.Application.Dtos;
public class CreateEmployeeDto : IBasicDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserProfile ProfileType { get; set; }

    public bool IsValid()
    {
        return Validate().Count == 0;
    }

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Name))
            errors.Add("O nome é obrigatório.");

        if (!StringUtils.IsValidEmail(Email))
            errors.Add("O e-mail informado é inválido.");

        if (!Enum.IsDefined(typeof(UserProfile), ProfileType))
            errors.Add("O tipo de perfil é inválido.");

        return errors;
    }
}
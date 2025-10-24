using ManiaDeLimpeza.Application.Common;
using ManiaDeLimpeza.Domain.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Dtos;

public class LoginDto : IBasicDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public bool IsValid()
    {
        return Validate().Count == 0;
    }

    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Email))
            errors.Add("E-mail é obrigatório.");
        else if (!Email.IsValidEmail())
            errors.Add("E-mail inválido.");

        if (string.IsNullOrWhiteSpace(Password))
            errors.Add("Senha é obrigatória.");

        return errors;
    }
}
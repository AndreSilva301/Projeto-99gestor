using ManiaDeLimpeza.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ManiaDeLimpeza.Application.Dtos
{
    public class RegisterUserDto
    {
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public bool AcceptTerms { get; set; }

        public List<string> Validate()
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Nome é obrigatório.");

            if (string.IsNullOrWhiteSpace(CompanyName))
                errors.Add("Nome da empresa é obrigatório.");

            if (string.IsNullOrWhiteSpace(Email))
                errors.Add("E-mail é obrigatório.");
            else if (!IsValidEmail(Email))
                errors.Add("E-mail inválido.");

            if (string.IsNullOrWhiteSpace(Password))
                errors.Add("Senha é obrigatória.");
            else if (Password.Length < 6)
                errors.Add("Senha deve ter pelo menos 6 caracteres.");

            if (!IsValidPhone(Phone))
                errors.Add("Telefone inválido. Deve conter entre 9 e 11 dígitos, podendo incluir espaços, parênteses, traços e o sinal de mais.");
            
            if (Password != ConfirmPassword)
                errors.Add("Senha e confirmação de senha não são iguais.");

            if(Phone.Length > 20)
                errors.Add("Telefone deve ter no máximo 20 caracteres.");

            if (!AcceptTerms)
                errors.Add("É necessário aceitar os termos de uso.");

            return errors;
        }

        private bool IsValidEmail(string email)
        {
            // Simple but effective regex for most valid email formats
            var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            // Allow digits, spaces, parentheses, dashes, and plus sign
            var pattern = @"^[\d\s\-\+\(\)]+$";
            if (!Regex.IsMatch(phone, pattern))
                return false;

            // Count digits only (ignore formatting characters)
            int digitCount = phone.Count(char.IsDigit);
            return digitCount >= 9 && digitCount <= 11;
        }

    }
}

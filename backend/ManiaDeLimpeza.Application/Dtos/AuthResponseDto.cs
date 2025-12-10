using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Dtos; 
public class AuthResponseDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string BearerToken { get; set; } = string.Empty;

    public AuthResponseDto() { }

    public AuthResponseDto(User user, string? bearerToken = null)
    {
        Id = user.Id;
        CompanyId = user.CompanyId;
        Name = user.Name;
        Email = user.Email;
        BearerToken = bearerToken ?? string.Empty;
    }
}

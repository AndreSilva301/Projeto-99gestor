using ManiaDeLimpeza.Domain.Entities;
using System.Globalization;

namespace ManiaDeLimpeza.Application.Dtos;
public class CreateEmployeeDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public UserProfile ProfileType { get; set; }

    /*
    if (dto.ProfileType == UserProfile.SystemAdmin)
    return BadRequest("Não é permitido criar usuários com perfil SystemAdmin.");
    */ // FAZER ESSA VALIDAÇÃO NO CONTROLER
}
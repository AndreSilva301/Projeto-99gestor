using ManiaDeLimpeza.Domain.Entities;
using System.Globalization;

namespace ManiaDeLimpeza.Application.Dtos;
public class CreateEmployeeDto
{
    public string Name { get; set; }
    public string Email { get; set; }
    public UserProfile ProfileType { get; set; }
}
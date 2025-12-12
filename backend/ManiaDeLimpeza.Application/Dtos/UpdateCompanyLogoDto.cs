using Microsoft.AspNetCore.Http;

namespace ManiaDeLimpeza.Application.Dtos;
public class UpdateCompanyLogoDto
{
    public IFormFile File { get; set; }
}

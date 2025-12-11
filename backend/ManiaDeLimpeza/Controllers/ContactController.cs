using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ManiaDeLimpeza.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ContactController : ControllerBase
{
    private readonly IContactService _contactService;

    public ContactController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> SendContact([FromBody] ContactRequestDto dto)
    {
        await _contactService.ProcessContactAsync(dto);

        return Ok(new { message = "Contato enviado com sucesso!" });
    }
} 
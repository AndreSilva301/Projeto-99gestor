using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Infrastructure.Exceptions;


namespace ManiaDeLimpeza.Application.Services;
public class LeadService : ILeadService
{
    private readonly ILeadRepository _leadRepository;

    public LeadService(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<string> CaptureLeadAsync(LeadCaptureDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new BusinessException("O nome é obrigatório.");

        var lead = new Lead
        {
            Name = dto.Name,
            Phone = dto.Phone,
             
        };

        await _leadRepository.AddAsync(lead);
        await _leadRepository.SaveChangesAsync();

        return "Lead capturado com sucesso";
    }
}

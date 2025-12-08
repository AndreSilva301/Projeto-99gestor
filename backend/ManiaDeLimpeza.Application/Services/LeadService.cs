using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using ManiaDeLimpeza.Domain.Exceptions;


namespace ManiaDeLimpeza.Application.Services;
public class LeadService : ILeadService, IScopedDependency
{
    private readonly ILeadRepository _leadRepository;

    public LeadService(ILeadRepository leadRepository)
    {
        _leadRepository = leadRepository;
    }

    public async Task<Lead> CaptureLeadAsync(LeadCaptureRequestDto dto)
    {
        if (!dto.IsValid(out var errorMessage))
            throw new BusinessException(errorMessage);

        var lead = new Lead
        {
            Name = dto.Name,
            Phone = dto.Phone,
        };

        await _leadRepository.AddAsync(lead);

        return lead;
    }
}

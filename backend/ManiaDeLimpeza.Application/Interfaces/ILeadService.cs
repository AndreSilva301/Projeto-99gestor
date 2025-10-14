using ManiaDeLimpeza.Application.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Interfaces;
public interface ILeadService
{
    Task<string> CaptureLeadAsync(LeadCaptureDto dto);
}


using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Domain.Persistence;
public interface IPasswordResetRepository
{
    Task AddAsync(PasswordResetToken passwordResetToken);
    Task<PasswordResetToken?> GetByTokenAsync(string token);
    Task<PasswordResetToken?> GetLatestByEmailAsync(string email);
    
}

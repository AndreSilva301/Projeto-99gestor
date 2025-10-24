using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Domain.Persistence
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByEmailAsync(string email);
        Task<User> UpdateAsync(User user);
        Task DeleteAsync(int userId);
        Task<User?> GetByIdAsync(int id);
        Task<IEnumerable<User>> GetByCompanyIdAsync(int companyId, bool includeInactive = false);
    }
}

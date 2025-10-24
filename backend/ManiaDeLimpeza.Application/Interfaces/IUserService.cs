using ManiaDeLimpeza.Application.Dtos;
using ManiaDeLimpeza.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(User user, string password);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByCredentialsAsync(string email, string password);
        Task<User?> UpdatePasswordAsync(User user, string newPassword);
        Task<User?> GetByIdAsync(int id);
        Task<User> ChangePasswordAsync(string email, string currentPassword, string newPassword);
        Task<IEnumerable<User>> GetUsersByCompanyIdAsync(int companyId, bool includelnactive = false);
        Task<User?> CreateEmployeeAsync(string name, string email, int companyId);
        Task<User?> UpdateUserAsync(User user);
        Task<User?> DeactivateUserAsync(int userId);
        Task<User?> ReactivateUserAsync(int userId);

    }
}

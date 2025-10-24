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
        #region GET

        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByCredentialsAsync(string email, string password);
        Task<IEnumerable<User>> GetUsersByCompanyIdAsync(int companyId, bool includeInactive = false);

        #endregion


        #region CREATE
        Task<User> CreateUserAsync(User user, string password);
        Task<User?> CreateEmployeeAsync(string name, string email, int companyId);

        #endregion


        #region UPDATE

        Task<User?> UpdateUserAsync(User user);
        Task<User?> UpdatePasswordAsync(User user, string newPassword);
        Task DeactivateUserAsync(int userId);
        Task ReactivateUserAsync(int userId);
        Task<User> ChangePasswordAsync(string email, string currentPassword, string newPassword);

        #endregion


        #region DELETE

        #endregion

    }
}

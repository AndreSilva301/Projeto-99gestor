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
        Task<User> UpdateUserAsync(User user);
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByCredentialsAsync(string email, string password);
    }
}

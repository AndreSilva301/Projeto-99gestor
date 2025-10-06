using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using ManiaDeLimpeza.Infrastructure.Exceptions;
using ManiaDeLimpeza.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Application.Services
{
    public class UserService : IUserService, IScopedDependency
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> CreateUserAsync(User user)
        { 
            user.PasswordHash = PasswordHelper.Hash(user.PasswordHash, user);

            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            throw new NotImplementedException();
            //// If password was changed, re-hash it (example check)
            //if (!string.IsNullOrWhiteSpace(user.Password))
            //{
            //    user.Password = PasswordHelper.Hash(user.Password, user);
            //}

            //// Assuming the repo has an UpdateAsync method (add it if needed)
            //return await _userRepository.UpdateAsync(user);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User?> GetByCredentialsAsync(string email, string password)
        {
            throw new NotImplementedException();    
            //var user = await _userRepository.GetByEmailAsync(email);

            //if (user == null)
            //    return null;

            //if (!PasswordHelper.Verify(user.PasswordHash, password, user))
            //{
            //    return null;
            //}

            //if (!user.IsActive)
            //    throw new BusinessException("The user must be approved in order to login");

            //return PasswordHelper.Verify(user.PasswordHash, password, user) ? user : null;
        }
    }
}

using AutoMapper;
using ManiaDeLimpeza.Application.Dtos;
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
        private readonly ICompanyServices _companyServices;
        private readonly IMapper _mapper;

        public UserService(
            IUserRepository userRepository, 
            ICompanyServices companyServices,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _companyServices = companyServices;
            _mapper = mapper;
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);

            if(existingUser != null)
            {
                throw new BusinessException("A user with this email already exists.");
            }

            // Updates the password hash
            user.PasswordHash = PasswordHelper.Hash(password, user);

            // Cria o usuário
            var createdUser = await _userRepository.AddAsync(user);

            return createdUser;
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

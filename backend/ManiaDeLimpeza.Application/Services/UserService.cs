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
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null)
                throw new BusinessException("Usuário não encontrado.");

            if (string.IsNullOrWhiteSpace(user.Name))
                throw new BusinessException("O nome não pode estar vazio.");

            existingUser.Name = user.Name.Trim();
            return await _userRepository.UpdateAsync(existingUser);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User?> GetByCredentialsAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return null;

            var isValid = PasswordHelper.Verify(password, user.PasswordHash, user);

            if (!isValid)
                return null;

            return user;
        }

        public async Task<User?> UpdatePasswordAsync(User user, string newPassword)
        {
            user.PasswordHash = PasswordHelper.Hash(newPassword, user);
            return await _userRepository.UpdateAsync(user);
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new BusinessException("Usuário não encontrado.");

            var isValid = PasswordHelper.Verify(currentPassword, user.PasswordHash, user);
            if (!isValid)
                throw new BusinessException("Senha atual incorreta.");

            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null && existingUser.Id != user.Id)
                throw new BusinessException("A user with this email already exists.");

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length < 6)
                throw new BusinessException("A nova senha deve ter pelo menos 6 caracteres.");

            user.PasswordHash = PasswordHelper.Hash(newPassword, user);

            return await _userRepository.UpdateAsync(user);
        }
    }
}

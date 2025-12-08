using AutoMapper;
using ManiaDeLimpeza.Application.Common;
using ManiaDeLimpeza.Application.Interfaces;
using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using ManiaDeLimpeza.Domain.Exceptions;
using ManiaDeLimpeza.Infrastructure.Helpers;

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

            if (existingUser != null)
            {
                throw new BusinessException("A user with this email already exists.");
            }

            user.PasswordHash = PasswordHelper.Hash(password, user);

            var createdUser = await _userRepository.AddAsync(user);

            return createdUser;
        }
        
        public async Task<User> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BusinessException("O e-mail é obrigatório.");

            if (string.IsNullOrWhiteSpace(currentPassword))
                throw new BusinessException("A senha atual é obrigatória.");

            if (!StringUtils.IsValidPassword(newPassword))
                throw new BusinessException("A nova senha deve ter pelo menos 8 caracteres, contendo ao menos uma letra e um número.");

            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                throw new BusinessException("Usuário não encontrado.");

            var isValid = PasswordHelper.Verify(currentPassword, user.PasswordHash, user);
            if (!isValid)
                throw new BusinessException("Senha atual incorreta.");

            return await UpdatePasswordAsync(user, newPassword);
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

        public async Task<User> UpdatePasswordAsync(User user, string newPassword)
        {
            if (user == null || user.Id <= 0)
                throw new BusinessException("Usuário inválido.");

            var existingUser = await _userRepository.GetByIdAsync(user.Id);

            if (existingUser == null)
                throw new BusinessException("Usuário não encontrado.");

            existingUser.PasswordHash = PasswordHelper.Hash(newPassword, existingUser);

            return await _userRepository.UpdateAsync(existingUser);
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> UpdateUserAsync(User updatedUser)
        {
            if (updatedUser == null)
                throw new BusinessException("Usuário inválido.");

            if (updatedUser.Id <= 0)
                throw new BusinessException("ID do usuário inválido.");

            var existingUser = await _userRepository.GetByIdAsync(updatedUser.Id);

            if (existingUser == null)
                throw new BusinessException("Usuário não encontrado.");

            existingUser.Name = updatedUser.Name;

            return await _userRepository.UpdateAsync(existingUser);
        }

        public async Task<IEnumerable<User>> GetUsersByCompanyIdAsync(int companyId, bool includeInactive = false)
        {
            return await _userRepository.GetByCompanyIdAsync(companyId, includeInactive);
        }

        public async Task<User?> CreateEmployeeAsync(string name, string email, int companyId)
        {
            var user = new User
            {
                Name = name,
                Email = email,
                CompanyId = companyId,
                Profile = UserProfile.Employee,
                PasswordHash = PasswordHelper.Hash(Guid.NewGuid().ToString("N")[..8], new User())
            };

            return await _userRepository.AddAsync(user);
        }

        public async Task<User> DeactivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new BusinessException("Usuário não encontrado.");

            user.Profile = UserProfile.Inactive;

            await _userRepository.UpdateAsync(user);

            return user;
        }

        public async Task<User> ReactivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId)
                ?? throw new BusinessException("Usuário não encontrado.");

            user.Profile = UserProfile.Employee;
            await _userRepository.UpdateAsync(user);

            return user;
        }
    }
}

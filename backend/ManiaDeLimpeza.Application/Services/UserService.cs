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
        private readonly ICompanyRepository _companyRepository;

        public UserService(IUserRepository userRepository, ICompanyRepository companyRepository)
        {
            _userRepository = userRepository;
            _companyRepository = companyRepository;

        }

        public async Task<User> CreateUserAsync(User user, Company company)
        {
            // Cria a empresa primeiro
            var createdCompany = await _companyRepository.CreateAsync(company);
            try
            {
                // Associa a empresa ao usuário
                user.CompanyId = createdCompany.Id;
                user.Company = createdCompany;
                user.PasswordHash = PasswordHelper.Hash(user.PasswordHash, user);

                // Cria o usuário
                var createdUser = await _userRepository.AddAsync(user);

                return createdUser;
            }
            catch (Exception)
            {
                // Se der erro, remove a empresa criada
                await _companyRepository.DeleteAsync(createdCompany);

                // Se o usuário foi criado antes do erro, remova também
                if (user.Id > 0)
                    await _userRepository.DeleteAsync(user.Id);

                throw; // Repassa o erro para o controller
            }
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

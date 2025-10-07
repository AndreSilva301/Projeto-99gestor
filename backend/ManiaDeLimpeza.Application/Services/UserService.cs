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

        public UserService(IUserRepository userRepository, ICompanyServices companyServices)
        {
            _userRepository = userRepository;
            _companyServices = companyServices;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            Company associatedCompany = null!;
            try
            {
                associatedCompany = await _companyServices.CreateCompanyAsync(user.Company);
                // Associa a empresa ao usuário
                user.CompanyId = associatedCompany.Id;
                user.Company = associatedCompany;
                user.PasswordHash = PasswordHelper.Hash(user.PasswordHash, user);

                // Cria o usuário
                var createdUser = await _userRepository.AddAsync(user);

                return createdUser;
            }
            catch (Exception)
            {
                if (associatedCompany != null && associatedCompany.Id > 0)
                   await _companyServices.DeleteCompanyAsync(associatedCompany.Id);
                 
                if (user.Id > 0)
                    await _userRepository.DeleteAsync(user.Id);
                throw; 
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

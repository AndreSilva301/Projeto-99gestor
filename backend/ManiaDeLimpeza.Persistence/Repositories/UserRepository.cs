using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Persistence.Repositories
{
    public class UserRepository : IUserRepository, IScopedDependency
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(
            ApplicationDbContext context
        ) {
            _context = context;
        }

        public async Task<User> AddAsync(User user)
        {
            var userEntityEntry = await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return userEntityEntry.Entity;
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var allUsers = await _context.Users.ToListAsync();
            return allUsers;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email == email);
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null)
                throw new InvalidOperationException("User not found");

            // Update all properties from the passed-in object
            _context.Entry(existingUser).CurrentValues.SetValues(user);

            // Prevent password update if it’s null/empty
            if (string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                _context.Entry(existingUser).Property(u => u.PasswordHash).IsModified = false;
            }

            await _context.SaveChangesAsync();
            return existingUser;
        }
        public async Task DeleteAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
       
    }
}

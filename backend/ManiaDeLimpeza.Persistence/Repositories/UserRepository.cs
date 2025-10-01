using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using ManiaDeLimpeza.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (string.IsNullOrWhiteSpace(user.Password))
            {
                _context.Entry(existingUser).Property(u => u.Password).IsModified = false;
            }

            await _context.SaveChangesAsync();
            return existingUser;
        }

    }
}

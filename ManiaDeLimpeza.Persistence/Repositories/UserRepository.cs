using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Domain.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Persistence.Repositories
{
    public class UserRepository : IUserRepository
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
    }
}

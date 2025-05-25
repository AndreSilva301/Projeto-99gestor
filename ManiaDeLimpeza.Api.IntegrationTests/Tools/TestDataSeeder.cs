using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Infrastructure.Helpers;
using ManiaDeLimpeza.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tools
{
    public static class TestDataSeeder
    {
        private const string DefaultEmail = "testuser@example.com";
        public static string DefaultPassword = "Secure123";

        public static void SeedDefaultUser(ApplicationDbContext db)
        {
            var defaultUser = db.Users.SingleOrDefault(u => u.Email == DefaultEmail);
            if (defaultUser == null)
            {
                db.Users.Add(GetDefaultUser());
                db.SaveChanges();
            }
        }

        public static User GetDefaultUser()
        {
            var user =  new User
            {
                Name = "Test User",
                Email = DefaultEmail,
                IsActive = true,
            };

            user.Password = PasswordHelper.Hash(DefaultPassword, user);

            return user;
        }
    }
}

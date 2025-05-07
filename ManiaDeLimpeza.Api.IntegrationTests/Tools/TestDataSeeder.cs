using ManiaDeLimpeza.Domain.Entities;
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
        public static string DefaultPassword = "Secure123";

        public static void SeedDefaultUser(ApplicationDbContext db)
        {
            db.Users.Add(GetDefaultUser());
            db.SaveChanges();
        }

        public static User GetDefaultUser()
        {
            return new User
            {
                Name = "Test User",
                Email = "testuser@example.com",
                //This is the equivalent to the default password encripted
                Password = "AQAAAAIAAYagAAAAEJ9+gNnMeKheD20UwN5NjHTJr0pasHM1+m4Rc2XnczqpfkNlVHl+SHUt/99d2RpC8A=="
            };
        }
    }
}

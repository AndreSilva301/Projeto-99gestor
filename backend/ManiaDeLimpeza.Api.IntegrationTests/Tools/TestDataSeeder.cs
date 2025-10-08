using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Infrastructure.Helpers;
using ManiaDeLimpeza.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class TestDataSeeder
{
    public static string DefaultEmail = "testuser@example.com";
    public static string DefaultPassword = "Secure123";

    public static void SeedDefaultUser(ApplicationDbContext db)
    {
        var defaultUser = db.Users.SingleOrDefault(u => u.Email == DefaultEmail);
        if (defaultUser == null)
        {
            var company = new Company
            {
                Name = "Empresa Teste",
                CNPJ = "12345678000199"
            };

            var user = GetDefaultUser();
            user.Company = company;

            db.Companies.Add(company);
            db.Users.Add(user);
            db.SaveChanges();
        }
    }

    public static User GetDefaultUser()
    {
        return new User
        {
            Name = "Test User",
            Email = DefaultEmail,
            PasswordHash = PasswordHelper.Hash(DefaultPassword, null)
        };
    }
}

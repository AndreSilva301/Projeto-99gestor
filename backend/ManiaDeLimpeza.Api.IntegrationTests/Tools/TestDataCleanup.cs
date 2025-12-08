using ManiaDeLimpeza.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Api.IntegrationTests.Tools
{
    public static class TestDataCleanup
    {
        public static void ClearCompany(ApplicationDbContext db)
        {
            db.Companies.RemoveRange(db.Companies);
            db.SaveChanges();
        }

        public static void ClearUsers(ApplicationDbContext db)
        {
            db.Users.RemoveRange(db.Users);
            db.SaveChanges();
        }

        public static void ClearClients(ApplicationDbContext db)
        {
            db.Customers.RemoveRange(db.Customers);
            db.SaveChanges();
        }

        public static void ClearCustomers(ApplicationDbContext db)
        {
            // Clear relationships first (foreign key constraint)
            db.CustomerRelationships.RemoveRange(db.CustomerRelationships);
            db.Customers.RemoveRange(db.Customers);
            db.SaveChanges();
        }

        public static void ClearQuotes(ApplicationDbContext db)
        {
            db.Quotes.RemoveRange(db.Quotes);
            db.SaveChanges();
        }

    }
}

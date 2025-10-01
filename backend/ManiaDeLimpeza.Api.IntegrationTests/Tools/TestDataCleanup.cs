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
        public static void ClearUsers(ApplicationDbContext db)
        {
            db.Users.RemoveRange(db.Users);
            db.SaveChanges();
        }

        public static void ClearClients(ApplicationDbContext db)
        {
            db.Clients.RemoveRange(db.Clients);
            db.SaveChanges();
        }

        public static void ClearQuotes(ApplicationDbContext db)
        {
            db.Quotes.RemoveRange(db.Quotes);
            db.SaveChanges();
        }

    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Persistence.IntegrationTests.Tools
{
    public static class TestDbContextFactory
    {
        private static string? _connectionString;

        public static void InitializeConfiguration()
        {
            if (_connectionString != null)
                return;

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            _connectionString = config.GetConnectionString("TestDatabase");

            // Apply migrations just once
            using var db = CreateContext();
            db.Database.Migrate();
        }

        public static ApplicationDbContext CreateContext()
        {
            if (_connectionString == null)
                throw new InvalidOperationException("Configuration not initialized. Call InitializeConfiguration() first.");

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlServer(_connectionString)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}

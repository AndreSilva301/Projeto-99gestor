using ManiaDeLimpeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManiaDeLimpeza.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        /* For my own reference:
         *   Whenever I modify the entities (e.g., add properties, constraints, new entities),
         *   I need to create a new migration and apply it to the database.
         *   These are the commands to do so (run from the solution root):
         *
         *   1. Create a new migration (replace 'MigrationName' with a meaningful name):
         *      dotnet ef migrations add MigrationName --project ManiaDeLimpeza.Persistence --startup-project ManiaDeLimpeza
         *
         *   2. Apply the migration to the database:
         *      dotnet ef database update --project ManiaDeLimpeza.Persistence --startup-project ManiaDeLimpeza
         *
         *   Notes:
         *   - Ensure the DbContext and Fluent API configurations are correct before generating the migration.
         *   - Use 'dotnet ef migrations remove' to undo if a migration was added by mistake.
         */

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Client> Clients => Set<Client>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Fluent API config if needed

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id).ValueGeneratedOnAdd();

                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(c => c.Id)
                .ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Client>().OwnsOne(c => c.Address);
            modelBuilder.Entity<Client>().OwnsOne(c => c.Phone);
        }
    }
}

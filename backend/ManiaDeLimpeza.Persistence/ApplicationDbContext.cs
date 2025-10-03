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
         *   1. Create a new migration (replace '{MigrationName}' with a meaningful name):
         *      dotnet ef migrations add {MigrationName} --project ManiaDeLimpeza.Persistence --startup-project ManiaDeLimpeza
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

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<CustumerRelationship> CostumerRelationships => Set<CustumerRelationship>();
        public DbSet<Quote> Quotes => Set<Quote>();
        public DbSet<QuoteItem> QuoteItems => Set<QuoteItem>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Fluent API config if needed

            //User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd();

                entity.HasIndex(u => u.Email)
                    .IsUnique();
            });

            //Client
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(c => c.Id)
                    .ValueGeneratedOnAdd();

                entity.OwnsOne(c => c.Address);
                entity.OwnsOne(c => c.Phone, phone =>
                {
                    phone.Property(p => p.Mobile);
                    phone.Property(p => p.Landline);

                    phone.HasIndex(p => p.Mobile);
                    phone.HasIndex(p => p.Landline);
                });

                entity.HasIndex(c => c.Name);

            });

            //Quote
            modelBuilder.Entity<Quote>(entity =>
            {
                entity.HasKey(q => q.Id);
                entity.Property(q => q.TotalPrice).HasColumnType("decimal(18,2)");
                entity.Property(q => q.CashDiscount).HasColumnType("decimal(18,2)");
                entity.HasOne(q => q.Customer)
                    .WithMany()
                    .HasForeignKey(q => q.CostumerId);

                entity.HasOne(q => q.User)
                    .WithMany()
                    .HasForeignKey(q => q.UserId);

                entity.HasMany(q => q.QuoteItems)
                    .WithOne()
                    .HasForeignKey(li => li.QuoteId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //QuoteItem
            modelBuilder.Entity<QuoteItem>(entity =>
            {
                entity.HasKey(li => li.Id);
                entity.Property(li => li.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(li => li.TotalValue).HasColumnType("decimal(18,2)");
            });
        }
    }
}

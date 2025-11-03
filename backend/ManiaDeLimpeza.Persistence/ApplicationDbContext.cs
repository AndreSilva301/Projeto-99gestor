using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Persistence.Configurations;
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
        public DbSet<CustomerRelationship> CustomerRelationships => Set<CustomerRelationship>();
        public DbSet<Quote> Quotes => Set<Quote>();
        public DbSet<QuoteItem> QuoteItems => Set<QuoteItem>();
        public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
        public DbSet<Lead> Leads => Set<Lead>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CompanyConfiguration());
            base.OnModelCreating(modelBuilder);
            // Fluent API config if needed

            //Company
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id)
                    .ValueGeneratedOnAdd();

                // Company (1) -> (N) User
                entity.HasMany(c => c.Users)
                    .WithOne(u => u.Company)
                    .HasForeignKey(u => u.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Company (1) -> (N) Customer
                entity.HasMany<Customer>()
                     .WithOne(cu => cu.Company)
                    .HasForeignKey(cu => cu.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.Property(u => u.Id)
                    .ValueGeneratedOnAdd();

                entity.HasIndex(u => u.Email)
                    .IsUnique();
            });

            //Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(c => c.Id);
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

                // Customer (1) -> (N) CustomerRelationship
                entity.HasMany(c => c.CostumerRelationships)
                    .WithOne(cr => cr.Customer)
                    .HasForeignKey(cr => cr.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Customer (1) -> (N) Quote
                entity.HasMany(c => c.Quotes)
                    .WithOne(q => q.Customer)
                    .HasForeignKey(q => q.CostumerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            //CustomerRelationship
            modelBuilder.Entity<CustomerRelationship>(entity =>
            {
                entity.HasKey(cr => cr.Id);
                entity.Property(cr => cr.Id)
                    .ValueGeneratedOnAdd();
            });

            //Quote
            modelBuilder.Entity<Quote>(entity =>
            {
                entity.HasKey(q => q.Id);
                entity.Property(q => q.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(q => q.TotalPrice).HasColumnType("decimal(18,2)");
                entity.Property(q => q.CashDiscount).HasColumnType("decimal(18,2)");

                // Quote (N) -> (1) User
                entity.HasOne(q => q.User)
                    .WithMany()
                    .HasForeignKey(q => q.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Quote (1) -> (N) QuoteItem
                entity.HasMany(q => q.QuoteItems)
                    .WithOne()
                    .HasForeignKey(qi => qi.QuoteId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            //QuoteItem
            modelBuilder.Entity<QuoteItem>(entity =>
            {
                entity.HasKey(qi => qi.Id);
                entity.Property(qi => qi.Id)
                    .ValueGeneratedOnAdd();

                entity.Property(qi => qi.UnitPrice).HasColumnType("decimal(18,2)");
                entity.Property(qi => qi.TotalValue).HasColumnType("decimal(18,2)");
            });
           
        }
    }
}

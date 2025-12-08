using ManiaDeLimpeza.Domain.Entities;
using ManiaDeLimpeza.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;

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
            modelBuilder.ApplyConfiguration(new QuoteItemConfiguration());
            modelBuilder.ApplyConfiguration(new QuoteConfiguration());
            base.OnModelCreating(modelBuilder);
            // Fluent API config if needed

            //Company
            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).ValueGeneratedOnAdd();

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
                entity.Property(u => u.Id).ValueGeneratedOnAdd();

                entity.HasIndex(u => u.Email).IsUnique();
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
                entity.HasMany(c => c.CustomerRelationships)
                    .WithOne(cr => cr.Customer)
                    .HasForeignKey(cr => cr.CustomerId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Note: Customer -> Quote relationship is configured in QuoteConfiguration
            });

            //CustomerRelationship
            modelBuilder.Entity<CustomerRelationship>(entity =>
            {
                entity.HasKey(cr => cr.Id);
                entity.Property(cr => cr.Id)
                    .ValueGeneratedOnAdd();
            });

        }
    }
}

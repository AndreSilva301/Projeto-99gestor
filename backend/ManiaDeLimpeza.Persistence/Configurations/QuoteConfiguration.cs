using ManiaDeLimpeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManiaDeLimpeza.Persistence.Configurations;
public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).ValueGeneratedOnAdd();

        builder.Property(q => q.TotalPrice)
            .HasPrecision(18, 2);

        builder.Property(q => q.CashDiscount)
            .HasPrecision(18, 2);

        builder.Property(q => q.PaymentConditions)
            .HasMaxLength(500);

        builder.Property(q => q.CreatedAt)
            .IsRequired();

        builder.HasOne(q => q.Customer)
            .WithMany(cu => cu.Quotes)
            .HasForeignKey(q => q.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(q => q.User)
            .WithMany()
            .HasForeignKey(q => q.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure CompanyId as foreign key without navigation property
        builder.Property(q => q.CompanyId)
            .IsRequired();

        builder.HasOne<Company>()
            .WithMany()
            .HasForeignKey(q => q.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(q => q.QuoteItems)
            .WithOne()
            .HasForeignKey(qi => qi.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(q => q.CustomerId);
        builder.HasIndex(q => q.UserId);
        builder.HasIndex(q => q.CompanyId);
        builder.HasIndex(q => q.CreatedAt);
    }
}


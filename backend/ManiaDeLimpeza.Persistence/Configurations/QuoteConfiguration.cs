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
            .HasForeignKey(q => q.CustomerId);

        builder.HasOne(q => q.User)
            .WithMany()
            .HasForeignKey(q => q.UserId);

        builder.HasMany(q => q.QuoteItems)
            .WithOne(qi => qi.Quote)
            .HasForeignKey(qi => qi.QuoteId);

        builder.HasIndex(q => q.CustomerId);
        builder.HasIndex(q => q.UserId);
        builder.HasIndex(q => q.CreatedAt);
    }
}


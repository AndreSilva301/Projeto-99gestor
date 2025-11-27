using ManiaDeLimpeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace ManiaDeLimpeza.Persistence.Configurations;
public class QuoteItemConfiguration : IEntityTypeConfiguration<QuoteItem>
{
    public void Configure(EntityTypeBuilder<QuoteItem> builder)
    {
        builder.HasKey(qi => qi.Id);

        builder.Property(qi => qi.Quantity)
            .HasPrecision(18, 2);

        builder.Property(qi => qi.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(qi => qi.TotalPrice)
            .HasPrecision(18, 2);

        builder.Property(qi => qi.Description)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(qi => qi.CustomFields)
     .HasConversion(
         v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
         v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, (JsonSerializerOptions)null) ?? new Dictionary<string, string>())
     .HasColumnType("nvarchar(max)");

        builder.HasIndex(qi => qi.QuoteId);
        builder.HasIndex(qi => qi.Order);

        builder.HasOne<Quote>()
            .WithMany(q => q.QuoteItems)
            .HasForeignKey(qi => qi.QuoteId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
using ManiaDeLimpeza.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ManiaDeLimpeza.Persistence.Configurations;
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.CNPJ)
            .HasMaxLength(18);

        builder.OwnsOne(c => c.Address, address =>
        {
            address.Property(a => a.Street).HasMaxLength(150);
            address.Property(a => a.Number).HasMaxLength(10);
            address.Property(a => a.Complement).HasMaxLength(100);
            address.Property(a => a.Neighborhood).HasMaxLength(100);
            address.Property(a => a.City).HasMaxLength(100);
            address.Property(a => a.State).HasMaxLength(50);
            address.Property(a => a.ZipCode).HasMaxLength(15);
        });

        builder.OwnsOne(c => c.Phone, phone =>
        {
            phone.Property(p => p.Mobile).HasMaxLength(20);
            phone.Property(p => p.Landline).HasMaxLength(20);
        });
    }
}
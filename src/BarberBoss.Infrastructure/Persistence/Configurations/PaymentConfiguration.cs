using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBoss.Infrastructure.Persistence.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Method).HasConversion<int>();
        builder.Property(x => x.Status).HasConversion<int>();

        //builder.OwnsOne(x => x.Amount, m =>
        //{
        //    m.Property(p => p.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)");
        //    m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
        //});

        builder.Property(x => x.Provider).HasMaxLength(80);
        builder.Property(x => x.ExternalReference).HasMaxLength(120);

        builder.HasIndex(x => new { x.CompanyId, x.BranchId, x.Provider, x.ExternalReference });
    }
}

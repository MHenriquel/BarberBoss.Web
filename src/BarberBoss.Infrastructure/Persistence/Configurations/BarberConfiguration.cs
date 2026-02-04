using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBoss.Infrastructure.Persistence.Configurations;

public class BarberConfiguration : IEntityTypeConfiguration<Barber>
{
    public void Configure(EntityTypeBuilder<Barber> builder)
    {
        builder.ToTable("Barbers");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
        //builder.OwnsOne(x => x.DefaultCommission, c =>
        //{
        //    c.Property(p => p.Percentage).HasColumnName("DefaultCommissionPercentage").HasColumnType("decimal(5,4)");
        //    c.Property(p => p.FixedAmount).HasColumnName("DefaultCommissionFixedAmount").HasColumnType("decimal(18,2)");
        //});
        builder.HasIndex(x => new { x.CompanyId, x.BranchId, x.FullName });
    }
}

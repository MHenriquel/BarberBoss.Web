using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBoss.Infrastructure.Persistence.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(120).IsRequired();
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        //builder.OwnsOne(x => x.CommissionOverride, c =>
        //{
        //    c.Property(p => p.Percentage).HasColumnName("CommissionPercentage").HasColumnType("decimal(5,4)");
        //    c.Property(p => p.FixedAmount).HasColumnName("CommissionFixedAmount").HasColumnType("decimal(18,2)");
        //});
        builder.HasIndex(x => new { x.CompanyId, x.BranchId, x.Name });
    }
}

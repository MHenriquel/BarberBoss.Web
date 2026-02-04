using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBoss.Infrastructure.Persistence.Configurations;

public class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("Branches");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Timezone).HasMaxLength(80).IsRequired();
        builder.Property(x => x.AddressLine).HasMaxLength(300);
        builder.Property(x => x.Phone).HasMaxLength(50);
        builder.HasIndex(x => new { x.CompanyId, x.Name });
    }
}

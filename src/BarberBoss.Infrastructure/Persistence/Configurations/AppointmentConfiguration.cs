using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBoss.Infrastructure.Persistence.Configurations;

public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
{
    public void Configure(EntityTypeBuilder<Appointment> builder)
    {
        builder.ToTable("Appointments");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Status).HasConversion<int>();
        builder.Property(x => x.Notes).HasMaxLength(1000);

        builder.OwnsMany(x => x.Items, items =>
        {
            items.ToTable("AppointmentItems");
            items.WithOwner().HasForeignKey("AppointmentId");
            items.Property<Guid>("Id");
            items.HasKey("Id");
            items.Property(x => x.Name).HasMaxLength(120).IsRequired();
            items.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
        });

        builder.HasIndex(x => new { x.CompanyId, x.BranchId, x.BarberId, x.StartsAt, x.EndsAt });
    }
}

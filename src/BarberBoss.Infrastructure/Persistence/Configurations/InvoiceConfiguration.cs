using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBoss.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        builder.HasKey(x => x.Id);

        //builder.OwnsOne(x => x.Subtotal, m =>
        //{
        //    m.Property(p => p.Amount).HasColumnName("SubtotalAmount").HasColumnType("decimal(18,2)");
        //    m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
        //});
        //builder.OwnsOne(x => x.Discount, m =>
        //{
        //    m.Property(p => p.Amount).HasColumnName("DiscountAmount").HasColumnType("decimal(18,2)");
        //    m.Property(p => p.Currency).HasColumnName("DiscountCurrency").HasMaxLength(3);
        //});
        //builder.OwnsOne(x => x.Total, m =>
        //{
        //    m.Property(p => p.Amount).HasColumnName("TotalAmount").HasColumnType("decimal(18,2)");
        //    m.Property(p => p.Currency).HasColumnName("TotalCurrency").HasMaxLength(3);
        //});
        //builder.OwnsOne(x => x.BarberCommission, m =>
        //{
        //    m.Property(p => p.Amount).HasColumnName("BarberCommissionAmount").HasColumnType("decimal(18,2)");
        //    m.Property(p => p.Currency).HasColumnName("BarberCommissionCurrency").HasMaxLength(3);
        //});

        builder.HasMany(x => x.Payments).WithOne().HasForeignKey(p => p.InvoiceId);
        builder.HasIndex(x => new { x.CompanyId, x.BranchId, x.AppointmentId }).IsUnique();
    }
}

using BarberBoss.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BarberBoss.Infrastructure.Persistence.Configurations;

public class ExpenseConfiguration : IEntityTypeConfiguration<Expense>
{
    public void Configure(EntityTypeBuilder<Expense> builder)
    {
        builder.ToTable("Expenses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Description).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Category).HasMaxLength(80);

        //builder.OwnsOne(x => x.Amount, m =>
        //{
        //    m.Property(p => p.Amount).HasColumnName("Amount").HasColumnType("decimal(18,2)");
        //    m.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3);
        //});

        builder.HasIndex(x => new { x.CompanyId, x.BranchId, x.OccurredAt });
    }
}

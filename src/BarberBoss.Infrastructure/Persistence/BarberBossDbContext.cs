using BarberBoss.Application.Abstractions.Persistence;
using BarberBoss.Domain.Entities;
using BarberBoss.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.Persistence;

public class BarberBossDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IUnitOfWork
{
    public BarberBossDbContext(DbContextOptions<BarberBossDbContext> options) : base(options) { }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Branch> Branches => Set<Branch>();
    public DbSet<Barber> Barbers => Set<Barber>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<Appointment> Appointments => Set<Appointment>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Expense> Expenses => Set<Expense>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {




        modelBuilder.Entity<Barber>()
                .Ignore(b => b.DefaultCommission);


        modelBuilder.Entity<Expense>()
            .Ignore(e => e.Amount);

        modelBuilder.Entity<Invoice>()
            .Ignore(i => i.BarberCommission);

        modelBuilder.Entity<Invoice>()
            .Ignore(i => i.Discount);
        modelBuilder.Entity<Invoice>()
            .Ignore(i => i.Subtotal);
        modelBuilder.Entity<Invoice>()
            .Ignore(i => i.Total);
        modelBuilder.Entity<Payment>()
            .Ignore(i => i.Amount);
        modelBuilder.Entity<Service>()
            .Ignore(i => i.CommissionOverride);







        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BarberBossDbContext).Assembly);
        base.OnModelCreating(modelBuilder);


        //Renomear tabelas do Identity para ficarem mais “limpas”
        modelBuilder.Entity<ApplicationUser>().ToTable("Users");
        modelBuilder.Entity<IdentityRole<Guid>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaims");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("UserTokens");

    }
}

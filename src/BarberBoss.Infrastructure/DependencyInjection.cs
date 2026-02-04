using BarberBoss.Application.Abstractions.Payments;
using BarberBoss.Application.Abstractions.Persistence;
using BarberBoss.Infrastructure.Payments;
using BarberBoss.Infrastructure.Persistence;
using BarberBoss.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BarberBoss.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddDbContext<BarberBossDbContext>(opt =>
            opt.UseSqlServer(cfg.GetConnectionString("Default")));

        services.AddScoped<DbContext>(sp => sp.GetRequiredService<BarberBossDbContext>());
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<BarberBossDbContext>());

        services.AddSingleton<IPaymentGateway, FakePaymentGateway>();
        return services;
    }
}

using BarberBoss.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BarberBoss.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly DbContext _ctx;
    public Repository(DbContext ctx) => _ctx = ctx;

    public Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => _ctx.Set<T>().FindAsync([id], ct).AsTask();

    public Task AddAsync(T entity, CancellationToken ct = default)
        => _ctx.Set<T>().AddAsync(entity, ct).AsTask();

    public void Update(T entity) => _ctx.Set<T>().Update(entity);
    public void Remove(T entity) => _ctx.Set<T>().Remove(entity);
    public IQueryable<T> Query() => _ctx.Set<T>().AsQueryable();
}

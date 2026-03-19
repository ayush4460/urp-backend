using Microsoft.EntityFrameworkCore;
using URP.Domain.Repositories;
using URP.Infrastructure.Persistence;

namespace URP.Infrastructure.Repositories;

public abstract class BaseRepository<TEntity, TKey>(ApplicationDbContext db)
    : IRepository<TEntity, TKey> where TEntity : class
{
    protected readonly ApplicationDbContext _db  = db;
    protected readonly DbSet<TEntity>       _set = db.Set<TEntity>();

    public virtual async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
        => await _set.FindAsync(new object?[] { id }, ct);

    public virtual async Task AddAsync(TEntity entity, CancellationToken ct = default)
        => await _set.AddAsync(entity, ct);

    public virtual void Update(TEntity entity) => _set.Update(entity);
    public virtual void Remove(TEntity entity) => _set.Remove(entity);
}

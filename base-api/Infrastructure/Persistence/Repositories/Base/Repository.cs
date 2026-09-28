using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using POS.Domain.Abstractions.Repositories.Base;
using POS.Domain.Models.Base;

namespace POS.Persistence.Repositories.Base;

public abstract class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly DbSet<T> _dbSet;
    protected readonly ILogger _logger;

    protected Repository(DbSet<T> dbSet, ILogger logger)
    {
        _dbSet = dbSet;
        _logger = logger;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct)
    {
        if (ct.IsCancellationRequested) return new List<T>().AsEnumerable();
        return await _dbSet.ToListAsync(ct);
    }

    public virtual async Task<T> GetByIdAsync(Guid id, CancellationToken ct)
    {
        T? value = await AddIncludes(_dbSet.AsNoTracking()).FirstOrDefaultAsync(e => e.Id == id, ct);
        return value ?? throw new Exception();
    }

    public virtual ValueTask<EntityEntry<T>> CreateAsync(T entity, CancellationToken ct)
    {
        return _dbSet.AddAsync(entity, ct);
    }

    public virtual EntityEntry<T> Update(T entity)
    {
        return _dbSet.Update(entity);
    }

    public virtual async Task<(T, EntityEntry<T>)> UpdateAsync(Guid entityId, CancellationToken ct)
    {
        var value = await GetByIdAsync(entityId, ct);
        return (value, _dbSet.Update(value));
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        _dbSet.Remove(await GetByIdAsync(id, ct));
    }

    protected virtual IQueryable<T> AddIncludes(IQueryable<T> query)
    {
        return query;
    }
}

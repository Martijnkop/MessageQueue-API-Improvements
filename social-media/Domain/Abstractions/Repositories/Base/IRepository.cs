using Microsoft.EntityFrameworkCore.ChangeTracking;
using Social.Common.Primitives.ServiceLifetimes;
using Social.Domain.Models.Base;

namespace Social.Domain.Abstractions.Repositories.Base;

public interface IRepository<T> : ITransient where T : Entity
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);
    Task<T> GetByIdAsync(Guid id, CancellationToken ct);
    ValueTask<EntityEntry<T>> CreateAsync(T entity, CancellationToken ct);
    Task<(T, EntityEntry<T>)> UpdateAsync(Guid entityId, CancellationToken ct);
    EntityEntry<T> Update(T entity);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

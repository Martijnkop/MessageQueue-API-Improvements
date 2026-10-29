using Microsoft.EntityFrameworkCore.ChangeTracking;
using Social.Common.Primitives.ServiceLifetimes;
using Social.Domain.Models.Base;

namespace Social.Domain.Abstractions.Repositories.Base;

public interface IRepository<T> : ITransient where T : Entity
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);
    Task<T> GetByIdAsync(Guid id, CancellationToken ct);
}

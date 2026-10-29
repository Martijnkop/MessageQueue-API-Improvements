using Social.Common.Primitives.ServiceLifetimes;

namespace Social.Business.Abstractions.Services.Base;

public interface IService<T> : ITransient
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);
    Task<T> GetByIdAsync(Guid id, CancellationToken ct);
}

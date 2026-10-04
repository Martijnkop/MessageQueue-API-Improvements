using Social.Common.Primitives.ServiceLifetimes;

namespace Social.Business.Abstractions.Services.Base;

public interface IService<T, TCreate, TEdit> : ITransient
{
    Task<IEnumerable<T>> GetAllAsync(CancellationToken ct);
    Task<T> GetByIdAsync(Guid id, CancellationToken ct);
    Task<T> CreateAsync(TCreate createModel, CancellationToken ct);
    Task<T> EditAsync(Guid id, TEdit editDTO, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

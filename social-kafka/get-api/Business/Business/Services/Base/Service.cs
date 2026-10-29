using Microsoft.Extensions.Logging;
using Social.Business.Abstractions.Services.Base;
using Social.Domain.Abstractions;
using Social.Domain.Abstractions.Repositories.Base;
using Social.Domain.Models.Base;

namespace Social.Business.Services.Base;

public abstract class Service<T> : IService<T>
    where T : Entity
{
    private readonly IRepository<T> _repository;
    private readonly ILogger _logger;

    public Service(IRepository<T> repository, ILogger logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public Task<IEnumerable<T>> GetAllAsync(CancellationToken ct)
    {
        _logger.LogInformation("Getting all entities of type {EntityType}", typeof(T).Name);
        return _repository.GetAllAsync(ct);
    }

    public Task<T> GetByIdAsync(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Getting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
        return _repository.GetByIdAsync(id, ct);
    }
}

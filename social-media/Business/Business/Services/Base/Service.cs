using Microsoft.Extensions.Logging;
using Social.Business.Abstractions.Services.Base;
using Social.Domain.Abstractions;
using Social.Domain.Abstractions.Repositories.Base;
using Social.Domain.Models.Base;

namespace Social.Business.Services.Base;

public abstract class Service<T, TCreate, TEdit> : IService<T, TCreate, TEdit>
    where T : Entity
{
    private readonly IRepository<T> _repository;
    private readonly IUserRepository _userRepository;
    protected readonly IUnitOfWork _unitOfWork;
    private readonly ILogger _logger;

    public Service(IRepository<T> repository, IUserRepository userRepository, IUnitOfWork unitOfWork, ILogger logger)
    {
        _repository = repository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
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

    public async Task<T> CreateAsync(TCreate entity, Guid keycloakId, CancellationToken ct)
    {
        _logger.LogInformation("Creating entity of type {EntityType}", typeof(T).Name);
        Guid userId = await _userRepository.GetIdByKeycloakIdAsync(keycloakId, ct) ?? throw new NotImplementedException();
        var value = await _repository.CreateAsync(Map(entity, userId), ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return await GetByIdAsync(value.Entity.Id, ct);
    }

    public async Task<T> EditAsync(Guid id, TEdit entity, CancellationToken ct)
    {
        _logger.LogInformation("Editing entity of type {EntityType} with ID {Id}", typeof(T).Name, id);

        (var value, var valueTask) = await _repository.UpdateAsync(id, ct);
        Update(value, entity);
        await _unitOfWork.SaveChangesAsync(ct);

        return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        _logger.LogInformation("Deleting entity of type {EntityType} with ID {Id}", typeof(T).Name, id);
        await _repository.DeleteAsync(id, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    protected abstract T Update(T value, TEdit entity);
    protected abstract T Map(TCreate entity, Guid userId);
}

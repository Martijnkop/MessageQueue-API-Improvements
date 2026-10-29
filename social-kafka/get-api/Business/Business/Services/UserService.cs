namespace Social.Business.Services;

public class UserService : Service<User>, IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository, IUnitOfWork unitOfWork, ILogger<UserService> logger) : base(repository, logger)
    {
        _repository = repository;
    }

    public Task<User?> Me(Guid kratosId, CancellationToken ct) => _repository.GetByKratosIdAsync(kratosId, ct);
}

namespace Social.Business.Services;

public class UserService : Service<User, CreateUserDTO, EditUserDTO>, IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository, IUnitOfWork unitOfWork, ILogger<UserService> logger) : base(repository, repository, unitOfWork, logger)
    {
        _repository = repository;
    }

    public Task<User?> Me(Guid kratosId, CancellationToken ct) => _repository.GetByKratosIdAsync(kratosId, ct);
    protected override User Map(CreateUserDTO entity, Guid userId) => throw new NotImplementedException();

    protected override User Update(User value, EditUserDTO entity) => value with {
        Username = entity.Username ?? value.Username,
    };
}

namespace Social.Business.Services;

public class UserService : Service<User, CreateUserDTO, EditUserDTO>, IUserService
{
    public UserService(IUserRepository repository, IUnitOfWork unitOfWork, ILogger<UserService> logger) : base(repository, repository, unitOfWork, logger)
    {
    }

    protected override User Map(CreateUserDTO entity, Guid userId) => throw new NotImplementedException();

    protected override User Update(User value, EditUserDTO entity) => value with {
        Username = entity.Username ?? value.Username,
    };
}

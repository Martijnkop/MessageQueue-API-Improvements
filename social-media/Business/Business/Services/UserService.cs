namespace Social.Business.Services;

public class UserService : Service<User, CreateUserDTO, EditUserDTO>, IUserService
{
    public UserService(IUserRepository repository, IUnitOfWork unitOfWork, ILogger<UserService> logger) : base(repository, unitOfWork, logger)
    {
    }

    protected override User Map(CreateUserDTO entity) => new User
    {
        Id = Guid.CreateVersion7(),
        CreatedTime = DateTime.UtcNow,
        Username = entity.Username
    };

    protected override User Update(User value, EditUserDTO entity) => value with {
        Username = entity.Username ?? value.Username,
    };
}

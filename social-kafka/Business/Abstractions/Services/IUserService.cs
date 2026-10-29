namespace Social.Business.Abstractions.Services;

public interface IUserService : IService<User, CreateUserDTO, EditUserDTO>
{
    Task<User?> Me(Guid kratosId, CancellationToken ct);
}

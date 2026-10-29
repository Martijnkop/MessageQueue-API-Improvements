namespace Social.Business.Abstractions.Services;

public interface IUserService : IService<User>
{
    Task<User?> Me(Guid kratosId, CancellationToken ct);
}

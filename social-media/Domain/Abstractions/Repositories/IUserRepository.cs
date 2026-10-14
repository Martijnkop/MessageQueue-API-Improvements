using Social.Domain.Abstractions.Repositories.Base;
using Social.Domain.Models;

namespace Social.Domain.Abstractions.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<bool> ExistsByKeycloakIdAsync(Guid keycloakId, CancellationToken ct);
    Task<Guid?> GetIdByKeycloakIdAsync(Guid keycloakId, CancellationToken ct);
}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Social.Domain.Abstractions.Repositories;
using Social.Domain.Models;
using Social.Persistence.Repositories.Base;

namespace Social.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDataStore dataStore, ILogger<UserRepository> logger) : base(dataStore.Users, logger)
    {
    }

    public async Task CreateAsync(User user, CancellationToken ct) => await _dbSet.AddAsync(user, ct);

    public Task<bool> ExistsByKeycloakIdAsync(Guid keycloakId, CancellationToken ct)
    {
        return _dbSet.AnyAsync(u => u.KeycloakId == keycloakId, ct);
    }

    public Task<User?> GetByKratosIdAsync(Guid kratosId, CancellationToken ct) => _dbSet.Where(u => u.KeycloakId == kratosId).FirstOrDefaultAsync(ct);

    public Task<Guid?> GetIdByKeycloakIdAsync(Guid keycloakId, CancellationToken ct)
    {
        return _dbSet
            .Where(u => u.KeycloakId == keycloakId)
            .Select(u => (Guid?)u.Id)
            .FirstOrDefaultAsync(ct);
    }
}

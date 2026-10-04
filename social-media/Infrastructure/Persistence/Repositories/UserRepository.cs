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
}

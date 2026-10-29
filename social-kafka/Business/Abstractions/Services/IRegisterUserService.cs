using System.Security.Claims;

namespace Social.Business.Abstractions.Services;

public interface IRegisterUserService
{
    Task EnsureRegisteredAsync(ClaimsPrincipal user, CancellationToken requestAborted);
}

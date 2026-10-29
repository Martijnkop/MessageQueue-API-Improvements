using Social.Common.Primitives.ServiceLifetimes;
using System.Security.Claims;

namespace Social.Business.Services;

public class RegisterUserService : IRegisterUserService, ITransient
{
    private readonly IUserRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RegisterUserService(IUserRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task EnsureRegisteredAsync(ClaimsPrincipal claimsUser, CancellationToken ct)
    {
        var sub = claimsUser.FindFirst("sub") ?? claimsUser.FindFirst(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(sub?.Value, out var keycloakId)) return;

        if (await _repository.ExistsByKeycloakIdAsync(keycloakId, ct)) return;

        var user = new User
        {
            Id = Guid.CreateVersion7(),
            CreatedTime = DateTime.UtcNow,
            KeycloakId = keycloakId,
            Username = claimsUser.FindFirst(ClaimTypes.Email)?.Value ?? "",
        };

        await _repository.CreateAsync(user, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }
}

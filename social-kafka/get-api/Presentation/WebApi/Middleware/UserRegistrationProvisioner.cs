using Microsoft.AspNetCore.Http;
using Social.Business.Abstractions.Services;

namespace Social.WebApi.Middleware;

public class UserRegistrationProvisioner
{
    private readonly RequestDelegate _next;
    public UserRegistrationProvisioner(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IRegisterUserService service)
    {
        if (context.User.Identity?.IsAuthenticated == true) // This is dumb, but it is a 'bool?'
        {
            await service.EnsureRegisteredAsync(context.User, context.RequestAborted);
        }

        await _next(context);
    }
}

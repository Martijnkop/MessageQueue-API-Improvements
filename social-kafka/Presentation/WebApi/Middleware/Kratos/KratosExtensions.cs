using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Social.Presentation.WebApi.Middleware.Kratos;
using Social.WebApi.Middleware.Kratos.Models;
using System.Security.Claims;
using System.Text.Json;

namespace Social.WebApi.Middleware.Kratos;

public static class KratosExtensions
{
    public static IServiceCollection AddKratosAuthentication(
        this IServiceCollection services, Action<KratosAuthOptions> configure)
    {
        var opts = new KratosAuthOptions();
        configure(opts);

        services.AddHttpClient("kratos", c =>
        {
            c.BaseAddress = new Uri(opts.PublicUrl.TrimEnd('/') + "/");
            c.Timeout = opts.TimeOut;
        });

        services.AddAuthentication(KratosDefaults.Scheme)
            .AddScheme<KratosAuthOptions, KratosAuthHandler>(KratosDefaults.Scheme, configure);

        services.AddAuthorizationBuilder()
            .AddPolicy(KratosPolicies.Verified, p => p
                .AddAuthenticationSchemes(KratosDefaults.Scheme)
                .RequireAuthenticatedUser()
                .RequireClaim(KratosDefaults.ClaimEmailVerified, "true"))
            .AddPolicy(KratosPolicies.Aal2, p => p
                .AddAuthenticationSchemes(KratosDefaults.Scheme)
                .RequireAuthenticatedUser()
                .RequireClaim(KratosDefaults.ClaimAal, "aal2"));

        return services;
    }

    /// <summary>Raw Kratos session JSON (identity, traits, devices, ...). Null if not authenticated.</summary>
    public static JsonElement? GetKratosSession(this HttpContext ctx) =>
        ctx.Items.TryGetValue(KratosDefaults.ItemSession, out var s) && s is JsonElement el ? el : null;

    /// <summary>Kratos identity id (use as your user foreign key).</summary>
    public static string? GetKratosIdentityId(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.NameIdentifier);
}

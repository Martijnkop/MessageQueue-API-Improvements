using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace Social.Presentation.WebApi.Middleware.Kratos;

public class KratosAuthHandler : AuthenticationHandler<KratosAuthOptions>
{
    private readonly IHttpClientFactory _http;

    public KratosAuthHandler(
        IOptionsMonitor<KratosAuthOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IHttpClientFactory http) : base(options, logger, encoder)
    {
        _http = http;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        // Retrieve cookie & token
        var cookie = Request.Headers.Cookie.ToString();
        var bearer = Request.Headers.Authorization.ToString();
        var token = Request.Headers["X-Session-Token"].ToString();

        if (string.IsNullOrEmpty(token) &&
            bearer.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            token = bearer["Bearer ".Length..].Trim();
        }

        if (string.IsNullOrEmpty(cookie) && string.IsNullOrEmpty(token))
            return AuthenticateResult.NoResult();

        // Build and send request to kratos
        using var req = new HttpRequestMessage(HttpMethod.Get, "sessions/whoami");
        req.Headers.Accept.ParseAdd("application/json");

        if (!string.IsNullOrEmpty(cookie)) req.Headers.TryAddWithoutValidation("Cookie", cookie);
        if (!string.IsNullOrEmpty(token)) req.Headers.TryAddWithoutValidation("X-Session-Token", token);

        HttpResponseMessage res;
        try
        {
            res = await _http.CreateClient("kratos").SendAsync(req, Context.RequestAborted);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            Logger.LogError(ex, "Kratos whoami request failed");
            Context.Items[KratosDefaults.ItemFailure] = "unavailable";
            return AuthenticateResult.Fail("Kratos unavailable");
        }

        using (res)
        {
            if (res.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return AuthenticateResult.NoResult();

            if (res.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                // Session exists but needs 2FA for this request
                Context.Items[KratosDefaults.ItemFailure] = "aa12_required";
                return AuthenticateResult.Fail("aa12 required");
            }

            if (!res.IsSuccessStatusCode)
            {
                Logger.LogError("Kratos whoami returned {Status}", (int)res.StatusCode);
                Context.Items[KratosDefaults.ItemFailure] = "unavailable";
                return AuthenticateResult.Fail("Kratos error");
            }

            // Valid user from here
            using var doc = await JsonDocument.ParseAsync(
                await res.Content.ReadAsStreamAsync(Context.RequestAborted),
                cancellationToken: Context.RequestAborted);
            var session = doc.RootElement;

            // TEST
            Console.WriteLine(JsonConvert.SerializeObject(session));

            if (!session.TryGetProperty("active", out var active) || !active.GetBoolean())
                return AuthenticateResult.NoResult();

            var identity = session.GetProperty("identity");
            var identityId = identity.GetProperty("id").GetString();

            var claims = new List<Claim>
            {
                new (ClaimTypes.NameIdentifier, identityId!),
                new (KratosDefaults.ClaimsSessionId, session.GetProperty("id").GetString() ?? ""),
                new (KratosDefaults.ClaimAal,
                     session.TryGetProperty("authenticator_assurance_level", out var aal)
                        ? aal.GetString() ?? "aal1" : "aal1"),
            };

            if (identity.TryGetProperty("traits", out var traits) &&
                traits.ValueKind == JsonValueKind.Object &&
                traits.TryGetProperty(Options.EmailTrait, out var email) &&
                email.ValueKind == JsonValueKind.String)
            {
                claims.Add(new Claim(ClaimTypes.Email, email.GetString()!));
                claims.Add(new Claim(ClaimTypes.Name, email.GetString()!));
            }

            var verified = identity.TryGetProperty("verifiable_addresses", out var addrs) &&
                addrs.ValueKind == JsonValueKind.Array &&
                addrs.EnumerateArray().Any(a =>
                    a.TryGetProperty("verified", out var v) && v.GetBoolean());

            claims.Add(new Claim(KratosDefaults.ClaimEmailVerified, verified ? "true" : "false"));

            // Make the raw session available to endpoints (Clone: doc is disposed after this method)
            Context.Items[KratosDefaults.ItemSession] = session.Clone();

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
            return AuthenticateResult.Success(new AuthenticationTicket(principal, Scheme.Name));

        }
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = Context.Items[KratosDefaults.ItemFailure] switch
        {
            "aal2_required" => StatusCodes.Status403Forbidden,
            "unavailable" => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status401Unauthorized,
        };
        var error = Context.Items[KratosDefaults.ItemFailure] as string ?? "unauthenticated";
        await Response.WriteAsJsonAsync(new { error });
    }

    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        // Authorization policy failed (e.g. not verified / not AAL2)
        Response.StatusCode = StatusCodes.Status403Forbidden;
        await Response.WriteAsJsonAsync(new { error = "forbidden" });
    }

}
using Social.ServiceInstallers.Abstractions;
using Social.ServiceInstallers.Config;
using Social.WebApi.Config;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Social.WebApi.Middleware;
using Social.WebApi.Middleware.Kratos;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.InstallServicesFromAssembly(ServiceInstallerAssembly.Assembly, builder.Configuration);

builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
    .AddApplicationPart(WebApiAssembly.Assembly);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.MetadataAddress = builder.Configuration["Keycloak:MetadataAddress"] ?? throw new Exception();
//        options.Audience = builder.Configuration["Keycloak:Audience"] ?? throw new Exception();

//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidIssuer = builder.Configuration["Keycloak:Issuer"] ?? throw new Exception()
//        };

//        options.RequireHttpsMetadata = false;
//    });

builder.Services.AddKratosAuthentication(o =>
{
    o.PublicUrl = builder.Configuration["Kratos:PublicUrl"] ?? "http://kratos:4433";
});


builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue("EnableScalar", false))
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserRegistrationProvisioner>();

app.MapControllers();

app.Run();

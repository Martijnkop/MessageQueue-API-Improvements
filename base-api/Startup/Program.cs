using POS.ServiceInstallers.Abstractions;
using POS.ServiceInstallers.Config;
using POS.WebApi.Config;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.InstallServicesFromAssembly(ServiceInstallerAssembly.Assembly, builder.Configuration);

builder.Services.AddControllers(options => options.SuppressAsyncSuffixInActionNames = false)
    .AddApplicationPart(WebApiAssembly.Assembly);

builder.Services.AddRouting(options => options.LowercaseUrls = true);

var app = builder.Build();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue("EnableScalar", false))
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

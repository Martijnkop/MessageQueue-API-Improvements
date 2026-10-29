using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Social.Business.Config;
using Social.ServiceInstallers.Abstractions;

namespace Social.ServiceInstallers;

public class BusinessServiceInstallers : IServiceInstaller
{
    public void InstallService(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAsMatchingInterface(BusinessAssembly.Assembly);
    }
}

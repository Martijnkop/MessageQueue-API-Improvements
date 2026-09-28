using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POS.Business.Config;
using POS.ServiceInstallers.Abstractions;

namespace POS.ServiceInstallers;

public class BusinessServiceInstallers : IServiceInstaller
{
    public void InstallService(IServiceCollection services, IConfiguration configuration)
    {
        services.AddAsMatchingInterface(BusinessAssembly.Assembly);
    }
}

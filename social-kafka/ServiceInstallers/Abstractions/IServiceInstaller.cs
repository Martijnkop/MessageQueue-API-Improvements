using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Social.ServiceInstallers.Abstractions;

public interface IServiceInstaller
{
    public void InstallService(IServiceCollection services, IConfiguration configuration);
}

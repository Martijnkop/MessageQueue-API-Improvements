using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Social.Persistence;
using Social.Persistence.Config;
using Social.ServiceInstallers.Abstractions;

namespace Social.ServiceInstallers;

public class PersistenceServiceInstallers : IServiceInstaller
{
    public void InstallService(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDataStore>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddAsMatchingInterface(PersistenceAssembly.Assembly);
    }
}

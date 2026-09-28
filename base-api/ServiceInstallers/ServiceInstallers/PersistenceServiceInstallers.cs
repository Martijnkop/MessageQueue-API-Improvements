using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POS.Business.Config;
using POS.Persistence;
using POS.Persistence.Config;
using POS.ServiceInstallers.Abstractions;

namespace POS.ServiceInstallers;

public class PersistenceServiceInstallers : IServiceInstaller
{
    public void InstallService(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDataStore>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddAsMatchingInterface(PersistenceAssembly.Assembly);
    }
}

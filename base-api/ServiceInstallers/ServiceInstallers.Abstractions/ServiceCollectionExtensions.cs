using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Scrutor;
using POS.Common.Primitives.ServiceLifetimes;

namespace POS.ServiceInstallers.Abstractions;

public static class ServiceCollectionExtensions
{
    public static void InstallServicesFromAssembly(this IServiceCollection services, Assembly assembly, IConfiguration config)
    {
        var serviceInstallers = ServiceInstallerFactory.GetServiceInstallersFromAssembly(assembly).ToList();
        serviceInstallers.ForEach(installer => installer.InstallService(services, config));
    }

    private static IServiceCollection AddTypeAsMatchingInterface(this IServiceCollection services, Assembly assembly, Type type, ServiceLifetime serviceLifetime) =>
        services.Scan(scan =>
        {
            var selector = scan.FromAssemblies(assembly)
                .AddClasses(classes => classes.AssignableTo(type), false)
                .UsingRegistrationStrategy(RegistrationStrategy.Throw)
                .AsMatchingInterface()
                .WithLifetime(serviceLifetime);
        });

    public static IServiceCollection AddAsMatchingInterface(this IServiceCollection services, Assembly assembly) =>
        services
            .AddTypeAsMatchingInterface(assembly, typeof(ITransient), ServiceLifetime.Transient)
            .AddTypeAsMatchingInterface(assembly, typeof(IScoped), ServiceLifetime.Scoped)
            .AddTypeAsMatchingInterface(assembly, typeof(ISingleton), ServiceLifetime.Singleton);
}

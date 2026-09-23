using System.Reflection;

namespace POS.ServiceInstallers.Abstractions;

public class ServiceInstallerFactory
{
    internal static IEnumerable<IServiceInstaller> GetServiceInstallersFromAssembly(Assembly assembly)
    {
        Type serviceInstallerType = typeof(IServiceInstaller);

        IEnumerable<IServiceInstaller> serviceInstallers = assembly.DefinedTypes
            .Where(type => serviceInstallerType.IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            .Select(type => Activator.CreateInstance(type) as IServiceInstaller)!;

        return serviceInstallers;
    }
}

using System.Reflection;

namespace POS.ServiceInstallers.Config;

public class ServiceInstallerAssembly
{
    public static Assembly Assembly => Assembly.GetExecutingAssembly();
}

using System.Reflection;

namespace Social.ServiceInstallers.Config;

public class ServiceInstallerAssembly
{
    public static Assembly Assembly => Assembly.GetExecutingAssembly();
}

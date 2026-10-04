using System.Reflection;

namespace Social.Persistence.Config;

public static class PersistenceAssembly
{
    public static Assembly Assembly => Assembly.GetExecutingAssembly();
}

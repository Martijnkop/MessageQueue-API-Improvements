using System.Reflection;

namespace POS.Persistence.Config;

public static class PersistenceAssembly
{
    public static Assembly Assembly => Assembly.GetExecutingAssembly();
}

using System.Reflection;

namespace Social.Business.Config;

public static class BusinessAssembly
{
    public static Assembly Assembly => Assembly.GetExecutingAssembly();
}

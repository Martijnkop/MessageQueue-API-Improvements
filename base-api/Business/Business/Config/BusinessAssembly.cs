using System.Reflection;

namespace POS.Business.Config;

public static class BusinessAssembly
{
    public static Assembly Assembly => Assembly.GetExecutingAssembly();
}

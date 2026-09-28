using System.Reflection;

namespace POS.WebApi.Config;

public class WebApiAssembly
{
    public static Assembly Assembly => Assembly.GetExecutingAssembly();
}

using System.Reflection;

namespace Social.WebApi.Config;

public class WebApiAssembly
{
    public static Assembly Assembly => Assembly.GetExecutingAssembly();
}

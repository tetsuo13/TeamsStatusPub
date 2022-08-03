using System.Reflection;

namespace TeamsStatusPub.Services;

/// <summary>
/// Application info pulled from the assembly.
/// </summary>
public class AssemblyAppInfo : IAppInfo
{
    public string ApplicationName => GetAssemblyAttribute<AssemblyProductAttribute>((attribute) => attribute.Product);
    public string ApplicationPath => AppContext.BaseDirectory;
    public string Copyright => GetAssemblyAttribute<AssemblyCopyrightAttribute>((attribute) => attribute.Copyright);
    public string WebsiteUrl => GetAssemblyAttribute<AssemblyCompanyAttribute>((attribute) => attribute.Company);

    public string Version
    {
        get
        {
            var version = _assembly.GetName().Version;
            return $"Version {version?.Major}.{version?.Minor}.{version?.Build}";
        }
    }

    private readonly Assembly _assembly;

    /// <summary>
    /// Initializes a new instance of the AssemblyAppInfo class.
    /// </summary>
    public AssemblyAppInfo()
    {
        _assembly = Assembly.GetExecutingAssembly();
    }

    private string GetAssemblyAttribute<T>(Func<T, string> attributeProperty)
    {
        var attributes = _assembly.GetCustomAttributes(typeof(T), false);

        if (attributes.Length == 0)
        {
            return string.Empty;
        }

        return attributeProperty((T)attributes[0]);
    }
}

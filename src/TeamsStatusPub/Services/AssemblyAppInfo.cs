using System.Reflection;

namespace TeamsStatusPub.Services;

/// <summary>
/// Application info pulled from the assembly.
/// </summary>
public class AssemblyAppInfo : IAppInfo
{
    public string ApplicationName => GetAssemblyAttribute<AssemblyProductAttribute>((attribute) => attribute.Product);
    public string Copyright => GetAssemblyAttribute<AssemblyCopyrightAttribute>((attribute) => attribute.Copyright);
    public string WebsiteUrl => GetAssemblyAttribute<AssemblyCompanyAttribute>((attribute) => attribute.Company);

    public string Version
    {
        get
        {
            var version = _assembly.GetName().Version;

            // This should never happen.
            if (version is null)
            {
                return "Unknown";
            }

            var canonicalVersion = $"Version {version.Major}.{version.Minor}.{version.Build}";

            // CI build will set this with the build number.
            if (version.Revision != -1)
            {
                canonicalVersion += $" (Build {version.Revision})";
            }

            return canonicalVersion;
        }
    }

    private readonly Assembly _assembly;

    /// <summary>
    /// Initializes a new instance of the AssemblyAppInfo class.
    /// </summary>
    public AssemblyAppInfo() : this(Assembly.GetExecutingAssembly())
    {
    }

    public AssemblyAppInfo(Assembly assembly)
    {
        _assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
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

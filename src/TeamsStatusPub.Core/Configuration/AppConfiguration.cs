using Microsoft.Extensions.Configuration;

namespace TeamsStatusPub.Core.Configuration;

/// <summary>
/// Methods for application configuration -- not settings.
/// </summary>
public static class AppConfiguration
{
    /// <summary>
    /// The name of the file that contains app configuration.
    /// </summary>
    public const string SettingsFileName = "appsettings.json";

    /// <summary>
    /// Create an <see cref="IConfiguration"/> instance from the app's
    /// configuration file.
    /// </summary>
    /// <returns>An <see cref="IConfiguration"/> instance.</returns>
    public static IConfiguration Build() =>
        new ConfigurationBuilder()
            .AddJsonFile(SettingsFileName)
            .Build();
}

using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Settings.Configuration;

namespace TeamsStatusPub.Core.Configuration;

/// <summary>
/// Extension methods that add logging.
/// </summary>
public static class LoggingConfiguration
{
    /// <summary>
    /// Gets the default logger that was created from calling
    /// <see cref="CreateDefaultLogger"/>.
    /// </summary>
    public static ILogger Logger => Log.Logger;

    /// <summary>
    /// Creates the default logger.
    /// </summary>
    public static void CreateDefaultLogger()
    {
        // Need to explicitly specify assemblies that contain sinks otherwise
        // an exception is thrown when launching as a single-file app.
        var readerOptions = new ConfigurationReaderOptions(typeof(FileLoggerConfigurationExtensions).Assembly);

        // Allow Serilog config to be modified by appsettings file at runtime.
        var configuration = AppConfiguration.Build();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File($"{nameof(TeamsStatusPub).ToLower()}.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
            .ReadFrom.Configuration(configuration, readerOptions)
            .CreateLogger();
    }

    public static void AddAppLogging(this IServiceCollection services) =>
        services.AddLogging(logBuilder => logBuilder.AddSerilog(dispose: true));
}

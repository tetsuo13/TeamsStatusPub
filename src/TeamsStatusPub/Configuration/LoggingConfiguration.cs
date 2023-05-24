using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Settings.Configuration;

namespace TeamsStatusPub.Configuration;

/// <summary>
/// Extension methods that add logging.
/// </summary>
internal static class LoggingConfiguration
{
    /// <summary>
    /// Adds logging into the <see cref="IHostBuilder"/> service collection.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder ConfigureAppLogging(this IHostBuilder builder)
    {
        return builder.UseSerilog();
    }

    /// <summary>
    /// Creates the default logger.
    /// </summary>
    public static void CreateDefaultLogger()
    {
        // Need to explicitly specify assemblies that contain sinks otherwise
        // an exception is thrown when launching as a single-file app.
        var readerOptions = new ConfigurationReaderOptions(typeof(FileLoggerConfigurationExtensions).Assembly);

        // Allow Serilog config to be modified by appsettings file at runtime.
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        Log.Logger = new LoggerConfiguration()
            .WriteTo.File($"{nameof(TeamsStatusPub).ToLower()}.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
            .ReadFrom.Configuration(configuration, readerOptions)
            .CreateLogger();
    }
}

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TeamsStatusPub.Core.Models;
using TeamsStatusPub.Core.Presenters;
using TeamsStatusPub.Core.Services;
using TeamsStatusPub.Core.Services.AvailabilityHandlers;
using TeamsStatusPub.Core.Services.AvailabilityHandlers.MicrosoftTeams;
using TeamsStatusPub.Core.Services.AvailabilityHandlers.MicrosoftTeams.FileSystemProviders;

namespace TeamsStatusPub.Core.Configuration;

/// <summary>
/// Extension methods for <see cref="IHostBuilder"/> that adds the application
/// services.
/// </summary>
public static class ServiceConfiguration
{
    /// <summary>
    /// Adds the application services into the service collection.
    /// </summary>
    /// <param name="builder">The <see cref="IHostBuilder"/> instance.</param>
    /// <returns>The <see cref="IHostBuilder"/>.</returns>
    public static IHostBuilder ConfigureAppServices(this IHostBuilder builder)
    {
        return builder.ConfigureServices((context, services) =>
        {
            services.Configure<RuntimeSettings>(context.Configuration.GetSection("Runtime"));

            services.AddTransient<IMainFormPresenter, MainFormPresenter>();
            services.AddTransient<IAboutFormPresenter, AboutFormPresenter>();

            services.AddTransient<IHttpProvider, HttpProvider>();

            services.AddSingleton<IAppInfo, AssemblyAppInfo>();

            using var sp = services.BuildServiceProvider();
            var runtimeSettings = sp.GetRequiredService<IOptions<RuntimeSettings>>();

            switch (runtimeSettings.Value.AvailabilityHandler)
            {
                case AvailabilitySystems.MicrosoftTeamsClassic:
                    services.AddTransient<IAvailabilityHandler, MicrosoftTeamsClassicHandler>();
                    break;

                case AvailabilitySystems.MicrosoftTeams:
                    services.AddTransient<IAvailabilityHandler, MicrosoftTeamsHandler>();
                    services.AddTransient<IFileSystemProvider, FileSystemWrapper>();
                    services.AddTransient<IDirectoryProvider, DirectoryWrapper>();
                    services.AddTransient<ILogDiscovery, LogDiscovery>();
                    break;

                default:
                    throw new NotImplementedException(
                        $"Unsupported 'AvailabilityHandler' value from appsettings.json: {runtimeSettings.Value.AvailabilityHandler}");
            }
        });
    }
}

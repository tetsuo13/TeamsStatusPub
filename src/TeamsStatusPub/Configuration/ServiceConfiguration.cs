using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using TeamsStatusPub.Models;
using TeamsStatusPub.Presenters;
using TeamsStatusPub.Services;
using TeamsStatusPub.Services.AvailabilityHandlers;
using TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams;
using TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams.FileSystemProviders;
using TeamsStatusPub.Views;

namespace TeamsStatusPub.Configuration;

/// <summary>
/// Extension methods for <see cref="IHostBuilder"/> that adds the application
/// services.
/// </summary>
internal static class ServiceConfiguration
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

            services.AddTransient<IMainForm, MainForm>();
            services.AddTransient<IAboutForm, AboutForm>();

            services.AddTransient<IHttpProvider, HttpProvider>();

            services.AddSingleton<IAppInfo, AssemblyAppInfo>();

            using var sp = services.BuildServiceProvider();
            var runtimeSettings = sp.GetRequiredService<IOptions<RuntimeSettings>>();

            switch (runtimeSettings.Value.AvailabilityHandler)
            {
                case MeetingSystems.MicrosoftTeamsClassic:
                    services.AddTransient<IAvailabilityHandler, MicrosoftTeamsClassicHandler>();
                    break;

                case MeetingSystems.MicrosoftTeams:
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

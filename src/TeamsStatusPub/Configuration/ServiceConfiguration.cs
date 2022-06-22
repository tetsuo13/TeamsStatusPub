using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeamsStatusPub.Models;
using TeamsStatusPub.Presenters;
using TeamsStatusPub.Services;
using TeamsStatusPub.Services.AvailabilityHandlers;
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
            services.AddTransient<IAvailabilityHandler, MicrosoftTeamsHandler>();
            services.AddSingleton<IAppInfo, AssemblyAppInfo>();
        });
    }
}

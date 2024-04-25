using Microsoft.Extensions.DependencyInjection;
using TeamsStatusPub.Core.Services.AvailabilityHandlers;
using TeamsStatusPub.Core.Services.AvailabilityHandlers.MicrosoftTeamsClassic;

namespace TeamsStatusPub.Core.Configuration;

public static partial class ServiceConfiguration
{
    /// <summary>
    /// Adds the services required for Microsoft Teams Classic into the service collection.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance.</param>
    /// <returns>The <see cref="IServiceCollection"/>.</returns>
    private static IServiceCollection ConfigureMicrosoftTeamsClassicServices(this IServiceCollection services)
    {
        services.AddSingleton<IMicrosoftTeamsClassicFactory, MicrosoftTeamsClassicFactory>(sp => new MicrosoftTeamsClassicFactory(sp));
        services.AddTransient<IAvailabilityHandler, MicrosoftTeamsClassicHandler>();

        return services;
    }
}

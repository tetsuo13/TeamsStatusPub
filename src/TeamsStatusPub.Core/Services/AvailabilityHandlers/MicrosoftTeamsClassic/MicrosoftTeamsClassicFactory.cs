using Microsoft.Extensions.DependencyInjection;

namespace TeamsStatusPub.Core.Services.AvailabilityHandlers.MicrosoftTeamsClassic;

internal class MicrosoftTeamsClassicFactory : IMicrosoftTeamsClassicFactory
{
    private readonly IServiceProvider _serviceProvider;

    public MicrosoftTeamsClassicFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public ILogFileReader CreateLogFileReader()
    {
        return _serviceProvider.GetRequiredService<ILogFileReader>();
    }
}

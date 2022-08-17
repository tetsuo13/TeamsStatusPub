using Microsoft.Extensions.Logging;
using Moq;
using TeamsStatusPub.Services.AvailabilityHandlers;

namespace TeamsStatusPub.Tests.Services.AvailabilityHandlers.MicrosoftTeams;

public abstract class MicrosoftTeamsHandlerTests
{
    protected static MicrosoftTeamsHandler GetHandler()
    {
        var logger = new Mock<ILogger<MicrosoftTeamsHandler>>();
        return new MicrosoftTeamsHandler(logger.Object);
    }
}

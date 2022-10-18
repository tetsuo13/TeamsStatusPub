using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams;

namespace TeamsStatusPub.Services.AvailabilityHandlers;

/// <summary>
/// Availability of user in the Microsoft Teams desktop application. This
/// doesn't handle the mobile or web versions.
/// </summary>
public class MicrosoftTeamsHandler : IAvailabilityHandler
{
    /// <summary>
    /// The result of the last log file processing.
    /// </summary>
    private bool _lastAvailability = true;

    /// <summary>
    /// The absolute path to the Teams log file.
    /// </summary>
    private readonly string _teamsLogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Microsoft", "Teams", "logs.txt");

    private readonly ILogger<MicrosoftTeamsHandler> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    /// Initializes a new instance of the MicrosoftTeamsHandler class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="serviceScopeFactory"></param>
    public MicrosoftTeamsHandler(ILogger<MicrosoftTeamsHandler> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }

    public bool IsAvailable()
    {
        if (!File.Exists(_teamsLogFilePath))
        {
            _logger.LogCritical("Couldn't find Teams log file at {logFilePath}", _teamsLogFilePath);
            throw new FileNotFoundException("Couldn't find Teams log file", _teamsLogFilePath);
        }

        var lastAvailabilityFromFile = FindLastAvailabilityFromLogFile(_teamsLogFilePath);

        if (lastAvailabilityFromFile.HasValue)
        {
            _lastAvailability = lastAvailabilityFromFile.Value;
        }

        return _lastAvailability;
    }

    private bool? FindLastAvailabilityFromLogFile(string logFilePath)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var logFileReader = scope.ServiceProvider.GetRequiredService<ILogFileReader>();

        var linesOfInterest = logFileReader.ReadLinesOfInterest(logFilePath);
        return LastAvailabilityFromLinesOfInterest(linesOfInterest);
    }

    internal static bool? LastAvailabilityFromLinesOfInterest(List<string> linesOfInterest)
    {
        if (linesOfInterest.Count == 0)
        {
            return null;
        }

        if (linesOfInterest[0].Contains(EventDataTokens.CallStarted) ||
            linesOfInterest[0].Contains(EventDataTokens.ScreenShareStarted))
        {
            return false;
        }

        return true;
    }
}

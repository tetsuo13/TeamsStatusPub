using Microsoft.Extensions.Logging;
using TeamsStatusPub.Core.Services.AvailabilityHandlers.MicrosoftTeamsClassic;

namespace TeamsStatusPub.Core.Services.AvailabilityHandlers;

/// <summary>
/// Availability of user in the Microsoft Teams Classic desktop application.
/// This doesn't handle the mobile or web versions.
/// </summary>
public class MicrosoftTeamsClassicHandler : IAvailabilityHandler
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

    private readonly ILogger<MicrosoftTeamsClassicHandler> _logger;
    private readonly IMicrosoftTeamsClassicFactory _microsoftTeamsClassicFactory;

    /// <summary>
    /// Initializes a new instance of the MicrosoftTeamsHandler class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="microsoftTeamsClassicFactory"></param>
    public MicrosoftTeamsClassicHandler(ILogger<MicrosoftTeamsClassicHandler> logger,
        IMicrosoftTeamsClassicFactory microsoftTeamsClassicFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _microsoftTeamsClassicFactory = microsoftTeamsClassicFactory ?? throw new ArgumentNullException(nameof(microsoftTeamsClassicFactory));
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
        var logFileReader = _microsoftTeamsClassicFactory.CreateLogFileReader();
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

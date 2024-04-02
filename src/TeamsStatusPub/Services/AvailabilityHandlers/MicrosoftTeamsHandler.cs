﻿using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams;
using TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams.FileSystemProviders;

namespace TeamsStatusPub.Services.AvailabilityHandlers;

/// <summary>
/// Availability of user in the Microsoft Teams desktop application.
/// </summary>
public partial class MicrosoftTeamsHandler : IAvailabilityHandler
{
    /// <summary>
    /// The availability from the last successful log parse. In case there's
    /// no successful parsing yet, use a default value of "available."
    /// </summary>
    private bool _lastAvailability;

    /// <summary>
    /// The absolute path to the Teams log directory. This is cached once
    /// found as it won't change throughout this application's lifetime.
    /// </summary>
    private string? _logDirectory = null;

    /// <summary>
    /// All statuses that should be considered as "not available." Case
    /// doesn't matter.
    /// </summary>
    private readonly string[] _statusesConsideredNotAvailable = ["Busy", "DoNotDisturb"];

    private readonly ILogger<MicrosoftTeamsHandler> _logger;
    private readonly IFileSystemProvider _fileSystemProvider;
    private readonly ILogDiscovery _logDiscovery;

    [GeneratedRegex("availability: ([a-zA-Z]+),")]
    private static partial Regex AvailabilityRegex();

    /// <summary>
    /// Initializes a new instance of the MicrosoftTeamsHandler class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="fileSystemProvider"></param>
    /// <param name="logDiscovery"></param>
    public MicrosoftTeamsHandler(ILogger<MicrosoftTeamsHandler> logger, IFileSystemProvider fileSystemProvider,
        ILogDiscovery logDiscovery)
        : this(logger, fileSystemProvider, logDiscovery, true)
    {
    }

    internal MicrosoftTeamsHandler(ILogger<MicrosoftTeamsHandler> logger, IFileSystemProvider fileSystemProvider,
        ILogDiscovery logDiscovery, bool lastAvailability)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystemProvider = fileSystemProvider ?? throw new ArgumentNullException(nameof(fileSystemProvider));
        _logDiscovery = logDiscovery ?? throw new ArgumentNullException(nameof(logDiscovery));
        _lastAvailability = lastAvailability;
    }

    public bool IsAvailable()
    {
        var statusChangeLine = MostRecentSignificantLogLine();

        if (statusChangeLine is null)
        {
            return _lastAvailability;
        }

        var statuses = AvailabilityRegex().Match(statusChangeLine);

        if (!statuses.Success)
        {
            return _lastAvailability;
        }

        var lineIndicatesBusy = statuses.Groups.Values

            // First element contains entire regex pattern. Ignore it.
            .Skip(1)

            // There may be more than one account logged into Teams. Check
            // each account's availability. The line indicates busy if there's
            // any status that indicates so.
            //
            // Note: Unclear if it's possible to have statuses not in sync.
            // Multiple accounts scenario was not tested.
            .Any(latestStatusGroup =>
            {
                return _statusesConsideredNotAvailable.Any(notAvailableStatus =>
                {
                    return notAvailableStatus.Equals(latestStatusGroup.Value,
                        StringComparison.InvariantCultureIgnoreCase);
                });
            });

        // If busy then not available.
        _lastAvailability = !lineIndicatesBusy;
        return _lastAvailability;
    }

    private string? MostRecentSignificantLogLine()
    {
        // It's possible that this program is launched before Teams is first
        // launched and has a chance to create the appropriate directories.
        // Try to find Teams every time this method is queried until it's
        // found.
        _logDirectory ??= _logDiscovery.FindLogDirectory();

        if (string.IsNullOrEmpty(_logDirectory))
        {
            _logger.LogInformation("Couldn't find log directory");
            return null;
        }

        // Have to discover what the latest log file is every time a query is
        // needed as it's likely to change during runtime -- new file every
        // day and even multiple files throughout the day.
        var logFile = _logDiscovery.FindLogPath(_logDirectory);

        if (string.IsNullOrEmpty(logFile))
        {
            _logger.LogInformation("Couldn't find log file");
            return null;
        }

        // Look for the last line that indicates a status change.
        return _fileSystemProvider
            .ReadAllLines(logFile)
            .LastOrDefault(x => x.Contains("New Global State Event: UserDataGlobalState"));
    }
}

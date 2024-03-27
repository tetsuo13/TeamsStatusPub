using Microsoft.Extensions.Logging;
using TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams.FileSystemProviders;

namespace TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams;

/// <summary>
/// Service to find the associated log file for Teams.
/// </summary>
public class LogDiscovery : ILogDiscovery
{
    private readonly ILogger<LogDiscovery> _logger;
    private readonly IFileSystemProvider _fileSystemProvider;

    /// <summary>
    /// Initializes a new instance of the LogDiscovery class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="fileSystemProvider"></param>
    public LogDiscovery(ILogger<LogDiscovery> logger, IFileSystemProvider fileSystemProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystemProvider = fileSystemProvider ?? throw new ArgumentNullException(nameof(fileSystemProvider));
    }

    public string? FindLogDirectory()
    {
        var packages = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Packages");

        if (!_fileSystemProvider.Directory.Exists(packages))
        {
            _logger.LogError("Couldn't find expected packages directory under AppData: {PackagesDirectory}", packages);
            return null;
        }

        // Will look something like this: MSTeams_3wflxb5d6aawd
        var msTeamsDirs = _fileSystemProvider.Directory.GetDirectories(packages, "MSTeams_*");

        if (msTeamsDirs.Length != 1)
        {
            _logger.LogError("Expected to find only one MSTeams directory but didn't: {@MsTeamsDirectories}", msTeamsDirs);
            return null;
        }

        var logsPath = Path.Combine(msTeamsDirs[0], "LocalCache", "Microsoft", "MSTeams", "Logs");

        if (!_fileSystemProvider.Directory.Exists(logsPath))
        {
            _logger.LogError("Couldn't find logs directory: {LogsDirectory}", logsPath);
            return null;
        }

        return logsPath;
    }

    public string? FindLogPath(string directory)
    {
        // Follows this pattern: MSTeams_2024-03-19_02-21-57.02.log
        var latestLogFile = _fileSystemProvider.Directory.GetFiles(directory, "MSTeams_*.log")
            .OrderByDescending(x => x)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(latestLogFile))
        {
            _logger.LogError("No log files found");
        }

        return latestLogFile;
    }
}

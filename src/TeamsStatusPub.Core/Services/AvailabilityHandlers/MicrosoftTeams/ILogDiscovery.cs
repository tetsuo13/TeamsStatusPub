namespace TeamsStatusPub.Core.Services.AvailabilityHandlers.MicrosoftTeams;

/// <summary>
/// Contract for log finder.
/// </summary>
public interface ILogDiscovery
{
    /// <summary>
    /// Find absolute path to the user's local Teams instance directory. Should
    /// reside under the file pattern
    /// <c>%LOCALAPPDATA%/Packages/MSTeams_*/LocalCache/Microsoft/MSTeams</c>
    /// which needs to be resolved at runtime and differs for each unique user.
    /// </summary>
    /// <returns>Absolute path to base directory or <see langword="null"/> if not found.</returns>
    string? FindLogDirectory();

    /// <summary>
    /// Find the absolute path to the most recent, and current, log file. File
    /// name uses pattern <c>MSTeams_yyyy-MM-dd_HH-mm-ss.counter.log</c> where
    /// date is when it was created and "counter" is a zero-based two digit
    /// counter that's incremented.
    /// </summary>
    /// <param name="directory">The absolute path to the log directory.</param>
    /// <returns>Absolute path to log file or <see langword="null"/> if not found.</returns>
    string? FindLogPath(string directory);
}

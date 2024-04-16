namespace TeamsStatusPub.Core.Services.AvailabilityHandlers.MicrosoftTeams.FileSystemProviders;

/// <summary>
/// Provider for file system-based methods. Exists mainly for unit testing.
/// </summary>
public interface IFileSystemProvider
{
    /// <summary>
    /// Directory operations.
    /// </summary>
    IDirectoryProvider Directory { get; }

    /// <summary>
    /// Read all lines from a file. Intended for small files -- where "small"
    /// is defined as a file under ~3MB. Will read files that are locked by
    /// other processes.
    /// </summary>
    /// <param name="path">The path to the file to read.</param>
    /// <returns>The lines from the file.</returns>
    IReadOnlyCollection<string> ReadAllLines(string path);
}

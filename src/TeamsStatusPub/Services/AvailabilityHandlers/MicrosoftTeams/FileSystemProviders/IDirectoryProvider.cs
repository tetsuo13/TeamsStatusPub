namespace TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams.FileSystemProviders;

/// <summary>
/// Wrapper for directory-based methods.
/// </summary>
public interface IDirectoryProvider
{
    bool Exists(string? path);
    string[] GetFiles(string path, string searchPattern);
    string[] GetDirectories(string path, string searchPattern);
}

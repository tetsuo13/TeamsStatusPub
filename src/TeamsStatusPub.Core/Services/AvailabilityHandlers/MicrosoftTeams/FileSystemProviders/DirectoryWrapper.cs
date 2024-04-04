namespace TeamsStatusPub.Core.Services.AvailabilityHandlers.MicrosoftTeams.FileSystemProviders;

/// <summary>
/// Wrapper around <see cref="Directory"/>.
/// </summary>
public class DirectoryWrapper : IDirectoryProvider
{
    public bool Exists(string? path) => Directory.Exists(path);
    public string[] GetDirectories(string path, string searchPattern) => Directory.GetDirectories(path, searchPattern);
    public string[] GetFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern);
}

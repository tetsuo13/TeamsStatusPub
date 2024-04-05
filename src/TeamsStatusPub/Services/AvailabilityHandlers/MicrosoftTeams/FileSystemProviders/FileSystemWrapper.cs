namespace TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams.FileSystemProviders;

/// <summary>
/// Wrapper around physical file system.
/// </summary>
internal class FileSystemWrapper : IFileSystemProvider
{
    public IDirectoryProvider Directory { get; }

    /// <summary>
    /// Initializes a new instance of the FileSystemWrapper class.
    /// </summary>
    public FileSystemWrapper()
    {
        Directory = new DirectoryWrapper();
    }

    public IReadOnlyCollection<string> ReadAllLines(string path)
    {
        var lines = new List<string>();

        // Expect the file to be locked by another process.
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using var reader = new StreamReader(fs);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            if (line is not null)
            {
                lines.Add(line);
            }
        }

        return lines;
    }
}

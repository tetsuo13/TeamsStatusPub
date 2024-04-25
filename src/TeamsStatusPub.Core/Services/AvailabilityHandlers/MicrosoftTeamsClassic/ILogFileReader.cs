namespace TeamsStatusPub.Core.Services.AvailabilityHandlers.MicrosoftTeamsClassic;

/// <summary>
/// Microsoft Teams log file reader.
/// </summary>
public interface ILogFileReader
{
    /// <summary>
    /// Reads the last select lines of interest which contain event data
    /// tokens. They are in reverse chronological order.
    /// </summary>
    /// <param name="logFilePath">The absolute path to the log file to read.</param>
    /// <returns>Collection of lines of interest.</returns>
    List<string> ReadLinesOfInterest(string logFilePath);
}

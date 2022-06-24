namespace TeamsStatusPub.Models;

/// <summary>
/// Application runtime settings from the <i>appsettings.json</i> file.
/// </summary>
public record RuntimeSettings
{
    /// <summary>
    /// The IP address to listen on.
    /// </summary>
    public string? ListenAddress { get; init; }

    /// <summary>
    /// The port to listen on.
    /// </summary>
    public int ListenPort { get; init; }

    /// <summary>
    /// The JSON object key name to use when sending the response object.
    /// </summary>
    public string? OutputAvailabilityKeyName { get; init; }
}

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
    /// The text to use when availability handler determines available.
    /// </summary>
    public string? OutputAvailableText { get; init; }

    /// <summary>
    /// The text to use when availability handler determines not available.
    /// </summary>
    public string? OutputNotAvailableText { get; init; }
}

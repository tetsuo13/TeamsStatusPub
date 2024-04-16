namespace TeamsStatusPub.Core.Services.AvailabilityHandlers.MicrosoftTeamsClassic;

/// <summary>
/// Relevant Event data tokens to parsing status.
/// </summary>
internal static class EventDataTokens
{
    /// <summary>
    /// The base event data token. A number concatenated with this token
    /// indicates a certain event has occurred, the most significant being
    /// <see cref="CallStarted"/>.
    /// </summary>
    public const string BaseToken = "eventData: s::;m::1;a::";

    /// <summary>
    /// A line with this token indicates that a call has been started.
    /// </summary>
    public const string CallStarted = $"{BaseToken}1";

    public const string ScreenShareStarted = $"{BaseToken}0";
}

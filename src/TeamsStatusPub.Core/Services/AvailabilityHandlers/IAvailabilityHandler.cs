namespace TeamsStatusPub.Core.Services.AvailabilityHandlers;

/// <summary>
/// Contract for determining the availability of a system.
/// </summary>
public interface IAvailabilityHandler
{
    /// <summary>
    /// Determine the current status of the system to see if it's available.
    /// </summary>
    /// <returns>Whether or not the system is available.</returns>
    bool IsAvailable();
}

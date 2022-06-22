namespace TeamsStatusPub.Services;

/// <summary>
/// HTTP protocol listener.
/// </summary>
public interface IHttpProvider
{
    /// <summary>
    /// Initiates the HTTP listener to wait for connections to act upon.
    /// </summary>
    /// <param name="availabilityHandler">
    /// The action to perform when a connection is received. The result of
    /// this action is used to generate a response.
    /// </param>
    Task Listen(Func<bool> availabilityHandler);
}

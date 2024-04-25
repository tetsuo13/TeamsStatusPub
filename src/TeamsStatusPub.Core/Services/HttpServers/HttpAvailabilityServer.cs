using System.Net;
using NetCoreServer;

namespace TeamsStatusPub.Core.Services.HttpServers;

public class HttpAvailabilityServer : HttpServer
{
    private readonly Func<bool> _availabilityHandler;
    private readonly IHttpFactory _httpFactory;

    private bool? _previousAvailabilityResult = null;

    /// <summary>
    /// Initializes a new instance of the HttpAvailabilityServer class.
    /// </summary>
    /// <param name="address"></param>
    /// <param name="port"></param>
    /// <param name="availabilityHandler"></param>
    public HttpAvailabilityServer(IPAddress address, int port,
        Func<bool> availabilityHandler, IHttpFactory httpFactory)
        : base(address, port)
    {
        _availabilityHandler = availabilityHandler ?? throw new ArgumentNullException(nameof(availabilityHandler));
        _httpFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
    }

    /// <summary>
    /// Method is called every time a request is received.
    /// </summary>
    protected override TcpSession CreateSession()
    {
        var currentAvailabilityResult = _availabilityHandler();
        var session = _httpFactory.CreateSession(this, _previousAvailabilityResult, currentAvailabilityResult);

        _previousAvailabilityResult = currentAvailabilityResult;

        return session;
    }
}

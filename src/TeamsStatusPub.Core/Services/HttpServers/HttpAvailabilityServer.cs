using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreServer;

namespace TeamsStatusPub.Core.Services.HttpServers;

public class HttpAvailabilityServer : HttpServer
{
    private readonly Func<bool> _availabilityHandler;
    private readonly ILogger<HttpAvailabilitySession> _sessionLogger;

    private bool? _previousAvailabilityResult = null;

    /// <summary>
    /// Initializes a new instance of the HttpAvailabilityServer class.
    /// </summary>
    /// <param name="serviceScopeFactory"></param>
    /// <param name="address"></param>
    /// <param name="port"></param>
    /// <param name="availabilityHandler"></param>
    public HttpAvailabilityServer(IServiceScopeFactory serviceScopeFactory,
        IPAddress address, int port, Func<bool> availabilityHandler)
        : base(address, port)
    {
        ArgumentNullException.ThrowIfNull(serviceScopeFactory);

        _availabilityHandler = availabilityHandler;

        using var scope = serviceScopeFactory.CreateScope();

        _sessionLogger = scope.ServiceProvider.GetRequiredService<ILogger<HttpAvailabilitySession>>();
    }

    /// <summary>
    /// Method is called every time a request is received.
    /// </summary>
    protected override TcpSession CreateSession()
    {
        var currentAvailabilityResult = _availabilityHandler();

        var session = new HttpAvailabilitySession(_sessionLogger, this,
            _previousAvailabilityResult, currentAvailabilityResult);

        _previousAvailabilityResult = currentAvailabilityResult;

        return session;
    }
}

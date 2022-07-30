using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreServer;
using TeamsStatusPub.Models;

namespace TeamsStatusPub.Services.HttpServers;

public class HttpAvailabilityServer : HttpServer
{
    private readonly Func<bool> _availabilityHandler;
    private readonly ILogger<HttpAvailabilitySession> _sessionLogger;
    private readonly IOptions<RuntimeSettings> _runtimeSettings;

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
        if (serviceScopeFactory is null)
        {
            throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        _availabilityHandler = availabilityHandler;

        using var scope = serviceScopeFactory.CreateScope();

        _sessionLogger = scope.ServiceProvider.GetRequiredService<ILogger<HttpAvailabilitySession>>();
        _runtimeSettings = scope.ServiceProvider.GetRequiredService<IOptions<RuntimeSettings>>();
    }

    /// <summary>
    /// Method is called every time a request is received.
    /// </summary>
    protected override TcpSession CreateSession()
    {
        var currentAvailabilityResult = _availabilityHandler();

        var session = new HttpAvailabilitySession(_sessionLogger, _runtimeSettings,
            this, _previousAvailabilityResult, currentAvailabilityResult);

        _previousAvailabilityResult = currentAvailabilityResult;

        return session;
    }
}

using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreServer;
using TeamsStatusPub.Models;

namespace TeamsStatusPub.Services.HttpServers;

public class HttpAvailabilityServer : HttpServer
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly Func<bool> _availabilityHandler;

    public HttpAvailabilityServer(IServiceScopeFactory serviceScopeFactory,
        IPAddress address, int port, Func<bool> availabilityHandler)
        : base(address, port)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _availabilityHandler = availabilityHandler;
    }

    protected override TcpSession CreateSession()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        return new HttpAvailabilitySession(scope.ServiceProvider.GetRequiredService<ILogger<HttpAvailabilitySession>>(),
            scope.ServiceProvider.GetRequiredService<IOptions<RuntimeSettings>>(), this, _availabilityHandler);
    }
}

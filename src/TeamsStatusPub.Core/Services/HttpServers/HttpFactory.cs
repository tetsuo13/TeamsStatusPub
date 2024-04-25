using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetCoreServer;

namespace TeamsStatusPub.Core.Services.HttpServers;

internal class HttpFactory : IHttpFactory
{
    private readonly IServiceProvider _serviceProvider;

    public HttpFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public HttpAvailabilityServer CreateServer(IPAddress address, int port, Func<bool> availabilityHandler)
    {
        return new HttpAvailabilityServer(address, port, availabilityHandler, this);
    }

    public HttpAvailabilitySession CreateSession(HttpServer server,
        bool? previousAvailabilityResult, bool currentAvailabilityResult)
    {
        return new HttpAvailabilitySession(_serviceProvider.GetRequiredService<ILogger<HttpAvailabilitySession>>(),
            server, previousAvailabilityResult, currentAvailabilityResult);
    }
}

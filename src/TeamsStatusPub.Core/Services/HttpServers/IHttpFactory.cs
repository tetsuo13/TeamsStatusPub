using System.Net;
using NetCoreServer;

namespace TeamsStatusPub.Core.Services.HttpServers;

public interface IHttpFactory
{
    HttpAvailabilityServer CreateServer(IPAddress address, int port, Func<bool> availabilityHandler);
    HttpAvailabilitySession CreateSession(HttpServer server, bool? previousAvailabilityResult, bool currentAvailabilityResult);
}

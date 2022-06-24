using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreServer;
using TeamsStatusPub.Models;

namespace TeamsStatusPub.Services.HttpServers;

public class HttpAvailabilitySession : HttpSession
{
    private readonly ILogger<HttpAvailabilitySession> _logger;
    private readonly IOptions<RuntimeSettings> _runtimeSettings;

    /// <summary>
    /// Function that will return a boolean to indicate if the user is
    /// available.
    /// </summary>
    private readonly Func<bool> _availabilityHandler;

    public HttpAvailabilitySession(ILogger<HttpAvailabilitySession> logger, IOptions<RuntimeSettings> runtimeSettings,
        HttpServer server, Func<bool> availabilityHandler)
        : base(server)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _runtimeSettings = runtimeSettings ?? throw new ArgumentNullException(nameof(runtimeSettings));
        _availabilityHandler = availabilityHandler;
    }

    protected override void OnReceivedRequest(HttpRequest request)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("{request}", request);
        }
        else
        {
            _logger.LogInformation("Request from {userAgent}", GetUserAgent(request));
        }

        var response = new JsonObject
        {
            // Intention is to return a true value if the user is busy, the
            // opposite of what the function returns.
            [_runtimeSettings.Value.OutputAvailabilityKeyName!] = _availabilityHandler() == false
        };

        _logger.LogInformation("Sending response {response}", response.ToJsonString());

        SendResponseAsync(Response.MakeGetResponse(response.ToJsonString(),
            $"{MediaTypeNames.Application.Json}; charset={Encoding.UTF8.HeaderName}"));
    }

    private static string GetUserAgent(HttpRequest request)
    {
        for (var i = 0; i < request.Headers; i++)
        {
            var header = request.Header(i);

            if (string.Compare(header.Item1, "user-agent", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return header.Item2;
            }
        }

        return "Undetermined";
    }
}

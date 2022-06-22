using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreServer;
using TeamsStatusPub.Models;

namespace TeamsStatusPub.Services.HttpServers;

public class HttpAvailabilitySession : HttpSession
{
    private readonly ILogger<HttpAvailabilitySession> _logger;
    private readonly IOptions<RuntimeSettings> _runtimeSettings;
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

        var isAvailable = _availabilityHandler();

        var availableText = isAvailable ? _runtimeSettings.Value.OutputAvailableText : _runtimeSettings.Value.OutputNotAvailableText;
        var response = JsonSerializer.Serialize(availableText);

        _logger.LogInformation("Sending response {response}", response);

        SendResponseAsync(Response.MakeGetResponse(response,
            $"{MediaTypeNames.Application.Json}; charset={Encoding.UTF8.HeaderName}"));
    }
}

using System.Net.Mime;
using System.Text;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NetCoreServer;
using TeamsStatusPub.Models;

namespace TeamsStatusPub.Services.HttpServers;

/// <summary>
/// Used to receive/send HTTP requests/responses.
/// </summary>
/// <remarks>
/// What's logged when receiving a request and sending the response depends on
/// the log level and availability result. When the log level is set to debug
/// or higher then every request made and the response will be logged. When
/// the log level is information or higher then only the request User-Agent
/// and response will be logged if the availability differs from the last
/// request that was made. This is to cut down on how much is logged under
/// normal circumstances. As a result of this contextual logging, the previous
/// availability result along with the current availability result are needed.
/// Since an instance of this class is created per request, those results must
/// be maintained by the consumer.
/// </remarks>
public class HttpAvailabilitySession : HttpSession
{
    internal const string UndeterminedUserAgent = "Undetermined";

    private readonly ILogger<HttpAvailabilitySession> _logger;
    private readonly IOptions<RuntimeSettings> _runtimeSettings;
    private readonly bool? _previousAvailabilityResult;
    private readonly bool _currentAvailabilityResult;

    /// <summary>
    /// Initializes a new instance of the HttpAvailabilitySession class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="runtimeSettings"></param>
    /// <param name="server"></param>
    /// <param name="availabilityHandler"></param>
    public HttpAvailabilitySession(ILogger<HttpAvailabilitySession> logger, IOptions<RuntimeSettings> runtimeSettings,
        HttpServer server, bool? previousAvailabilityResult, bool currentAvailabilityResult)
        : base(server)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _runtimeSettings = runtimeSettings ?? throw new ArgumentNullException(nameof(runtimeSettings));
        _previousAvailabilityResult = previousAvailabilityResult;
        _currentAvailabilityResult = currentAvailabilityResult;
    }

    protected override void OnReceivedRequest(HttpRequest request)
    {
        if (LogInformational())
        {
            _logger.LogInformation("Request from {userAgent}", GetUserAgent(request));
        }

        var response = new JsonObject
        {
            // Intention is to return a true value if the user is busy, the
            // opposite of what's in the current availability result.
            [_runtimeSettings.Value.OutputAvailabilityKeyName!] = _currentAvailabilityResult == false
        };

        if (LogInformational())
        {
            _logger.LogInformation("Sending response {response}", response.ToJsonString());
        }
        else
        {
            _logger.LogDebug("Sending response {response}", response.ToJsonString());
        }

        SendResponseAsync(Response.MakeGetResponse(response.ToJsonString(),
            $"{MediaTypeNames.Application.Json}; charset={Encoding.UTF8.HeaderName}"));
    }

    /// <summary>
    /// Only log info level messages if the availability on this request
    /// differs from that on the previous request.
    /// </summary>
    /// <returns>Whether to log or not.</returns>
    internal bool LogInformational() => _previousAvailabilityResult.HasValue &&
        _previousAvailabilityResult.Value != _currentAvailabilityResult;

    /// <summary>
    /// Gets the value of the User-Agent request header.
    /// </summary>
    /// <param name="request">The request.</param>
    /// <returns>
    /// The value of the header or <see cref="UndeterminedUserAgent"/> if the
    /// header wasn't found.
    /// </returns>
    internal static string GetUserAgent(HttpRequest request)
    {
        for (var i = 0; i < request.Headers; i++)
        {
            var header = request.Header(i);

            if (string.Compare(header.Item1, "user-agent", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                return header.Item2;
            }
        }

        return UndeterminedUserAgent;
    }
}

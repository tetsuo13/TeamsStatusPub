﻿using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TeamsStatusPub.Core.Models;
using TeamsStatusPub.Core.Services.HttpServers;

namespace TeamsStatusPub.Core.Services;

/// <summary>
/// HTTP protocol listener.
/// </summary>
/// <remarks>
/// Implementation avoids the more obvious choice of using
/// <see cref="HttpListener"/> for the HTTP server as it requires elevated
/// permissions in order to open a listening port, even above 1024.
/// </remarks>
public sealed class HttpProvider : IHttpProvider, IDisposable
{
    private readonly ILogger<HttpProvider> _logger;
    private readonly IOptions<RuntimeSettings> _runtimeSettings;
    private readonly IHttpFactory _httpFactory;

    private HttpAvailabilityServer? _server;
    private bool _disposedValue;

    /// <summary>
    /// Initializes a new instance of the HttpProvider class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="runtimeSettings"></param>
    /// <param name="httpFactory"></param>
    public HttpProvider(ILogger<HttpProvider> logger, IOptions<RuntimeSettings> runtimeSettings,
        IHttpFactory httpFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _runtimeSettings = runtimeSettings ?? throw new ArgumentNullException(nameof(runtimeSettings));
        _httpFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
    }

    internal string? VerifyReadyToListen(Func<bool>? availabilityHandler)
    {
        if (availabilityHandler is null)
        {
            return "No provider given";
        }

        if (!IPAddress.TryParse(_runtimeSettings.Value.ListenAddress, out _))
        {
            return $"Invalid listen address '{_runtimeSettings.Value.ListenAddress}' specified in {nameof(_runtimeSettings.Value.ListenAddress)} setting";
        }

        if (_runtimeSettings.Value.ListenPort < 1024 || _runtimeSettings.Value.ListenPort > 65535)
        {
            return $"{nameof(_runtimeSettings.Value.ListenPort)} should be between 1024-65535";
        }

        return null;
    }

    /// <inheritdoc/>
    public Task Listen(Func<bool>? availabilityHandler)
    {
        _logger.LogInformation("Listening for incoming connections...");

        var initializationError = VerifyReadyToListen(availabilityHandler);

        if (!string.IsNullOrEmpty(initializationError))
        {
            _logger.LogError("{error}", initializationError);
            throw new HttpListenerException((int)HttpStatusCode.BadGateway, initializationError);
        }

        _logger.LogInformation("Starting listener on http://{uri}:{port}...",
            _runtimeSettings.Value.ListenAddress, _runtimeSettings.Value.ListenPort);

        //_server = new HttpAvailabilityServer(_serviceScopeFactory,
        //    IPAddress.Parse(_runtimeSettings.Value.ListenAddress!),
        //    _runtimeSettings.Value.ListenPort, availabilityHandler!);

        _server = _httpFactory.CreateServer(IPAddress.Parse(_runtimeSettings.Value.ListenAddress!),
            _runtimeSettings.Value.ListenPort, availabilityHandler!);

        _server.Start();

        if (!_server.IsStarted)
        {
            _logger.LogError("Error starting listener");
            throw new HttpListenerException((int)HttpStatusCode.BadGateway, "Error starting listener");
        }

        return Task.CompletedTask;
    }

    private void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _server?.Stop();
            }
            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

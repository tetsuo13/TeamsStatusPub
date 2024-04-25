using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using TeamsStatusPub.Core.Models;
using TeamsStatusPub.Core.Services;
using TeamsStatusPub.Core.Services.HttpServers;
using Xunit;

namespace TeamsStatusPub.Core.UnitTests.Services;

public static class HttpProviderTests
{
    private const string ValidListenAddress = "127.0.0.1";
    private const int ValidListenPort = 28374;

    private static Func<bool>? ValidProvider => () => false;

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("632.12.545.3345")]
    public static void VerifyReadyToListen_InvalidListenAddress_ReturnsErrorMessage(string? listenAddress)
    {
        using var httpProvider = CreateProvider(listenAddress, ValidListenPort);
        Assert.NotNull(httpProvider.VerifyReadyToListen(ValidProvider));
    }

    [Fact]
    public static void VerifyReadyToListen_MissingAvailabilityHandler_ReturnsErrorMessage()
    {
        using var httpProvider = CreateProvider(ValidListenAddress, ValidListenPort);
        Assert.NotNull(httpProvider.VerifyReadyToListen(null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1023)]
    [InlineData(65536)]
    public static void VerifyReadyToListen_InvalidListenPort_ReturnsErrorMessage(int listenPort)
    {
        using var httpProvider = CreateProvider(ValidListenAddress, listenPort);
        Assert.NotNull(httpProvider.VerifyReadyToListen(ValidProvider));
    }

    [Fact]
    public static void VerifyReadyToListen_ValidOptions_ReturnsNull()
    {
        using var httpProvider = CreateProvider(ValidListenAddress, ValidListenPort);
        Assert.Null(httpProvider.VerifyReadyToListen(ValidProvider));
    }

    private static HttpProvider CreateProvider(string? listenAddress, int listenPort)
    {
        var logger = Substitute.For<ILogger<HttpProvider>>();
        var scopeFactory = Substitute.For<IHttpFactory>();
        var runtimeSettings = Options.Create(new RuntimeSettings
        {
            ListenAddress = listenAddress,
            ListenPort = listenPort
        });

        return new HttpProvider(logger, runtimeSettings, scopeFactory);
    }
}

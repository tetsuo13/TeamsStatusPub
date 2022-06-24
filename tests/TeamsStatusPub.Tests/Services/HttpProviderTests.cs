using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using TeamsStatusPub.Models;
using TeamsStatusPub.Services;
using Xunit;

namespace TeamsStatusPub.Tests.Services;

public static class HttpProviderTests
{
    private const string ValidListenAddress = "127.0.0.1";
    private const int ValidListenPort = 28374;
    private const string ValidOutputAvailabilityKeyName = "busy";

    private static Func<bool>? ValidProvider => () => false;

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("632.12.545.3345")]
    public static void VerifyReadyToListen_InvalidListenAddress_ReturnsErrorMessage(string? listenAddress)
    {
        using var httpProvider = CreateProvider(listenAddress, ValidListenPort, ValidOutputAvailabilityKeyName);
        Assert.NotNull(httpProvider.VerifyReadyToListen(ValidProvider));
    }

    [Fact]
    public static void VerifyReadyToListen_MissingAvailabilityHandler_ReturnsErrorMessage()
    {
        using var httpProvider = CreateProvider(ValidListenAddress, ValidListenPort, ValidOutputAvailabilityKeyName);
        Assert.NotNull(httpProvider.VerifyReadyToListen(null));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1023)]
    [InlineData(65536)]
    public static void VerifyReadyToListen_InvalidListenPort_ReturnsErrorMessage(int listenPort)
    {
        using var httpProvider = CreateProvider(ValidListenAddress, listenPort, ValidOutputAvailabilityKeyName);
        Assert.NotNull(httpProvider.VerifyReadyToListen(ValidProvider));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public static void VerifyReadyToListen_InvalidOutputAvailabilityKeyName_ReturnsErrorMessage(string? outputAvailabilityKeyName)
    {
        using var httpProvider = CreateProvider(ValidListenAddress, ValidListenPort, outputAvailabilityKeyName);
        Assert.NotNull(httpProvider.VerifyReadyToListen(ValidProvider));
    }

    [Fact]
    public static void VerifyReadyToListen_ValidOptions_ReturnsNull()
    {
        using var httpProvider = CreateProvider(ValidListenAddress, ValidListenPort, ValidOutputAvailabilityKeyName);
        Assert.Null(httpProvider.VerifyReadyToListen(ValidProvider));
    }

    private static HttpProvider CreateProvider(string? listenAddress, int listenPort, string? outputAvailabilityKeyName)
    {
        var logger = new Mock<ILogger<HttpProvider>>();
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var runtimeSettings = Options.Create(new RuntimeSettings
        {
            ListenAddress = listenAddress,
            ListenPort = listenPort,
            OutputAvailabilityKeyName = outputAvailabilityKeyName
        });

        return new HttpProvider(logger.Object, runtimeSettings, scopeFactory.Object);
    }
}

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
    public static TheoryData<Func<bool>?, string?, int, string?, string?, bool> VerifyReadyToListenData =>
        new()
        {
            { null,        null,        0, null,       null,   false },
            { () => false, null,        0, null,       null,   false },
            { null,        "127.0.0.1", 0, null,       null,   false },
            { () => false, "127.0.0.1", 0, null,       null,   false },
            { () => false, "127.0.0.1", 17493, null,       null,   false },
            { () => false, "127.0.0.1", 17493, "not-busy", null,   false },
            { () => false, "127.0.0.1", 17493, null,       "busy", false },
            { () => false, "127.0.0.1", 17493, "not-busy", "busy", true }
        };

    [Theory]
    [MemberData(nameof(VerifyReadyToListenData))]
    public static void VerifyReadyToListen(Func<bool>? provider, string? listenAddress, int listenPort,
        string? outputAvailableText, string? outputNotAvailableText, bool expectNullMessage)
    {
        var logger = new Mock<ILogger<HttpProvider>>();
        var scopeFactory = new Mock<IServiceScopeFactory>();
        var runtimeSettings = Options.Create(new RuntimeSettings
        {
            ListenAddress = listenAddress,
            ListenPort = listenPort,
            OutputAvailableText = outputAvailableText,
            OutputNotAvailableText = outputNotAvailableText
        });

        using var httpProvider = new HttpProvider(logger.Object, runtimeSettings, scopeFactory.Object);

        var actual = httpProvider.VerifyReadyToListen(provider);

        if (expectNullMessage)
        {
            Assert.Null(actual);
        }
        else
        {
            Assert.NotNull(actual);
        }
    }
}

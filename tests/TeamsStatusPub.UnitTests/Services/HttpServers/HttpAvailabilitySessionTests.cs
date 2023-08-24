using Microsoft.Extensions.Logging;
using NetCoreServer;
using NSubstitute;
using TeamsStatusPub.Services.HttpServers;
using Xunit;

namespace TeamsStatusPub.UnitTests.Services.HttpServers;

public static class HttpAvailabilitySessionTests
{
    [Fact]
    public static void GetUserAgent_MissingHeader_ReturnsFallback()
    {
        var request = DefaultHeaders();
        Assert.Equal(HttpAvailabilitySession.UndeterminedUserAgent, HttpAvailabilitySession.GetUserAgent(request));
    }

    [Fact]
    public static void GetUserAgent_ContainsHeader_ReturnsValue()
    {
        var userAgentValue = "HomeAssistant/2022.7.7 httpx/0.23.0 Python/3.10";

        var request = DefaultHeaders();
        request.SetHeader("User-Agent", userAgentValue);

        Assert.Equal(userAgentValue, HttpAvailabilitySession.GetUserAgent(request));
    }

    [Theory]
    [InlineData(null, false, false)]
    [InlineData(null, true, false)]
    [InlineData(false, false, false)]
    [InlineData(true, true, false)]
    [InlineData(false, true, true)]
    [InlineData(true, false, true)]
    public static void LogInformational(bool? previousAvailabilityResult,
        bool currentAvailabilityResult, bool expected)
    {
        var session = CreateSession(previousAvailabilityResult, currentAvailabilityResult);
        Assert.Equal(expected, session.LogInformational());
    }

    private static HttpRequest DefaultHeaders()
    {
        var request = new HttpRequest();
        request.SetHeader("Request method", "GET");
        request.SetHeader("Request URL", "/");
        request.SetHeader("Request protocol", "HTTP/1.1");
        request.SetHeader("Host", "192.168.1.1:12345");
        request.SetHeader("Accept", "*/*");
        request.SetHeader("Accept-Encoding", "gzip, deflate, br");
        request.SetHeader("Connection", "keep-alive");

        return request;
    }

    private static HttpAvailabilitySession CreateSession(bool? previousAvailabilityResult,
        bool currentAvailabilityResult)
    {
        var logger = Substitute.For<ILogger<HttpAvailabilitySession>>();
        var httpServer = new HttpServer("127.0.0.1", 12345);

        return new HttpAvailabilitySession(logger, httpServer,
            previousAvailabilityResult, currentAvailabilityResult);
    }
}

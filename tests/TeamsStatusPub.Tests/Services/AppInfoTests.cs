using System.IO;
using TeamsStatusPub.Services;
using Xunit;

namespace TeamsStatusPub.Tests.Services;

public static class AppInfoTests
{
    private static AssemblyAppInfo appInfo => new();

    [Fact]
    public static void ApplicationName_FromAssembly()
    {
        Assert.Equal("Teams Status Pub", appInfo.ApplicationName);
    }

    [Fact]
    public static void ApplicationPath_FromAssembly()
    {
        Assert.Contains(Path.Combine("TeamsStatusPub.Tests", "bin"), appInfo.ApplicationPath);
    }

    [Fact]
    public static void Copyright_FromAssembly()
    {
        Assert.StartsWith("Copyright © 20", appInfo.Copyright);
    }

    [Fact]
    public static void WebsiteUrl_FromAssembly()
    {
        Assert.StartsWith("https://www.github.com/tetsuo13", appInfo.WebsiteUrl);
    }

    [Fact]
    public static void Version_FromAssembly()
    {
        Assert.Matches(@"Version \d+\.\d+\.\d+", appInfo.Version);
    }
}

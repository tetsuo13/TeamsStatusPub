using TeamsStatusPub.Core.Services;
using Xunit;

namespace TeamsStatusPub.Core.IntegrationTests.Services;

public class AppInfoTests
{
    /// <summary>
    /// This will pull values from the TeamsStatusPub.Core project assembly.
    /// </summary>
    private readonly AssemblyAppInfo _appInfo = new();

    [Fact]
    public void ApplicationName_FromAssembly()
    {
        Assert.Equal("Teams Status Pub", _appInfo.ApplicationName);
    }

    [Fact]
    public void Copyright_FromAssembly()
    {
        Assert.StartsWith("Copyright (c) 2022", _appInfo.Copyright);
    }

    [Fact]
    public void WebsiteUrl_FromAssembly()
    {
        Assert.Equal("https://www.github.com/tetsuo13/TeamsStatusPub", _appInfo.WebsiteUrl);
    }
}

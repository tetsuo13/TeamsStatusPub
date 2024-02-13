using System;
using System.Reflection;
using NSubstitute;
using TeamsStatusPub.Services;
using Xunit;

namespace TeamsStatusPub.UnitTests.Services;

public static class AppInfoTests
{
    /// <summary>
    /// Generic instance that will pull values from the TeamsStatusPub project
    /// assembly. Create a more specialized instance for the test method
    /// to mock <see cref="Assembly"/> for the
    /// <see cref="AssemblyAppInfo(Assembly)"/> constructor.
    /// </summary>
    private static AssemblyAppInfo _appInfo => new();

    [Fact]
    public static void ApplicationName_FromAssembly()
    {
        Assert.Equal("Teams Status Pub", _appInfo.ApplicationName);
    }

    [Fact]
    public static void Copyright_FromAssembly()
    {
        // Note: copyright decade will need to be updated starting in the 30s
        // but hopefully we're still not using Teams by then...
        Assert.StartsWith("Copyright (c) 202", _appInfo.Copyright);
    }

    [Fact]
    public static void WebsiteUrl_FromAssembly()
    {
        Assert.StartsWith("https://www.github.com/tetsuo13", _appInfo.WebsiteUrl);
    }

    [Theory]
    [InlineData("1.2.3", "Version 1.2.3")]
    [InlineData("1.2.3.4", "Version 1.2.3 (Build 4)")]
    public static void Version_FromAssembly(string assemblyVersion, string expected)
    {
        var appInfo = GetAppInfoForVersion(assemblyVersion);
        Assert.Equal(expected, appInfo.Version);
    }

    [Fact]
    public static void Version_Null_FromAssembly()
    {
        var appInfo = GetAppInfoForVersion(null);
        Assert.Equal("Unknown", appInfo.Version);
    }

    private static AssemblyAppInfo GetAppInfoForVersion(string? assemblyVersion)
    {
        var assemblyName = new AssemblyName();

        if (assemblyVersion is not null)
        {
            assemblyName.Version = new Version(assemblyVersion);
        }

        var assemblyMock = Substitute.For<Assembly>();
        assemblyMock.GetName().Returns(assemblyName);
        return new(assemblyMock);
    }
}

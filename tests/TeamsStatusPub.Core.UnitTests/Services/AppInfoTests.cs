using System;
using System.Reflection;
using NSubstitute;
using TeamsStatusPub.Core.Services;
using Xunit;

namespace TeamsStatusPub.Core.UnitTests.Services;

public static class AppInfoTests
{
    [Theory]
    [InlineData(null, "")] // Missing attribute
    [InlineData(nameof(ApplicationName_FromAssembly), nameof(ApplicationName_FromAssembly))]
    public static void ApplicationName_FromAssembly(string? name, string expected)
    {
        var appInfo = GetAppInfoForAttribute<AssemblyProductAttribute>(name);
        Assert.Equal(expected, appInfo.ApplicationName);
    }

    [Theory]
    [InlineData(null, "")] // Missing attribute
    [InlineData($"Copyright (c) {nameof(Copyright_FromAssembly)}", $"Copyright (c) {nameof(Copyright_FromAssembly)}")]
    public static void Copyright_FromAssembly(string? copyright, string expected)
    {
        var appInfo = GetAppInfoForAttribute<AssemblyCopyrightAttribute>(copyright);
        Assert.Equal(expected, appInfo.Copyright);
    }

    [Theory]
    [InlineData(null, "")] // Missing attribute
    [InlineData($"https://www.example.com/{nameof(WebsiteUrl_FromAssembly)}", $"https://www.example.com/{nameof(WebsiteUrl_FromAssembly)}")]
    public static void WebsiteUrl_FromAssembly(string? websiteUrl, string expected)
    {
        var appInfo = GetAppInfoForAttribute<AssemblyCompanyAttribute>(websiteUrl);
        Assert.Equal(expected, appInfo.WebsiteUrl);
    }

    [Theory]
    [InlineData(null, "Unknown")] // Missing attribute
    [InlineData("1.2.3", "Version 1.2.3")]
    [InlineData("1.2.3.0", "Version 1.2.3 (Build 0)")]
    [InlineData("1.2.3.4", "Version 1.2.3 (Build 4)")]
    public static void Version_FromAssembly(string? assemblyVersion, string expected)
    {
        var appInfo = GetAppInfoForVersion(assemblyVersion);
        Assert.Equal(expected, appInfo.Version);
    }

    private static AssemblyAppInfo GetAppInfoForAttribute<T>(string? attributeValue)
        where T : Attribute
    {
        var assemblyAttribute = Activator.CreateInstance(typeof(T),
            [attributeValue ?? nameof(GetAppInfoForAttribute)]) as T;

        Assert.NotNull(assemblyAttribute);

        var assemblyMock = Substitute.For<Assembly>();
        assemblyMock.GetCustomAttributes(typeof(T), false)
            .Returns(attributeValue is null ? [] : [assemblyAttribute]);

        return new(assemblyMock);
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

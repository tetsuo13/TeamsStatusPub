using System;
using Microsoft.Extensions.Options;
using NSubstitute;
using TeamsStatusPub.Core.Models;
using TeamsStatusPub.Core.Services;
using TeamsStatusPub.Core.Services.AvailabilityHandlers;
using TeamsStatusPub.ViewModels;
using Xunit;

namespace TeamsStatusPub.UnitTests.ViewModels;

public static class AboutViewModelTests
{
    [Theory]
    [InlineData(AvailabilitySystems.MicrosoftTeams, true, "Teams: not busy")]
    [InlineData(AvailabilitySystems.MicrosoftTeams, false, "Teams: busy")]
    [InlineData(AvailabilitySystems.MicrosoftTeamsClassic, true, "Teams Classic: not busy")]
    [InlineData(AvailabilitySystems.MicrosoftTeamsClassic, false, "Teams Classic: busy")]
    public static void LastAvailabilitySystemStatus_ShowsSystemNameAndAvailability(AvailabilitySystems system,
        bool isAvailable, string expectedStatus)
    {
        var appInfo = Substitute.For<IAppInfo>();
        var runtimeSettings = Options.Create(new RuntimeSettings { AvailabilityHandler = system });
        var availabilityHandler = Substitute.For<IAvailabilityHandler>();
        availabilityHandler.IsAvailable().Returns(isAvailable);

        var presenter = new AboutViewModel(appInfo, runtimeSettings, availabilityHandler);

        Assert.Equal(expectedStatus, presenter.LastAvailabilitySystemStatus);
    }

    [Fact]
    public static void ListenUrl_RuntimeSettings_ShowsListenAddressAndPort()
    {
        var appInfo = Substitute.For<IAppInfo>();
        var runtimeSettings = Options.Create(new RuntimeSettings
        {
            ListenAddress = "10.11.12.13",
            ListenPort = 12345
        });
        var availabilityHandler = Substitute.For<IAvailabilityHandler>();

        var presenter = new AboutViewModel(appInfo, runtimeSettings, availabilityHandler);

        Assert.Equal($"http://{runtimeSettings.Value.ListenAddress}:{runtimeSettings.Value.ListenPort}/",
            presenter.ListenUrl);
    }

    [Fact]
    public static void ApplicationName_FromAppInfo()
    {
        const string expected = "2023 Acme Corp, LLC";
        var appInfo = Substitute.For<IAppInfo>();
        appInfo.ApplicationName.Returns(expected);

        TestPropertyFromAppInfo(appInfo, viewModel => Assert.Equal(expected, viewModel.ApplicationName));
    }

    [Fact]
    public static void Copyright_FromAppInfo()
    {
        const string expected = "2023 Acme Corp, LLC";
        var appInfo = Substitute.For<IAppInfo>();
        appInfo.Copyright.Returns(expected);

        TestPropertyFromAppInfo(appInfo, viewModel => Assert.Equal(expected, viewModel.Copyright));
    }

    [Fact]
    public static void WebsiteUrl_FromAppInfo()
    {
        const string expected = "https://youtu.be/dQw4w9WgXcQ";
        var appInfo = Substitute.For<IAppInfo>();
        appInfo.WebsiteUrl.Returns(expected);

        TestPropertyFromAppInfo(appInfo, viewModel => Assert.Equal(expected, viewModel.WebsiteUrl));
    }

    [Fact]
    public static void Version_FromAppInfo()
    {
        const string expected = "1.2.3";
        var appInfo = Substitute.For<IAppInfo>();
        appInfo.Version.Returns(expected);

        TestPropertyFromAppInfo(appInfo, viewModel => Assert.Equal(expected, viewModel.Version));
    }

    private static void TestPropertyFromAppInfo(IAppInfo appInfo, Action<AboutViewModel> value)
    {
        var runtimeSettings = Options.Create(new RuntimeSettings());
        var availabilityHandler = Substitute.For<IAvailabilityHandler>();

        var presenter = new AboutViewModel(appInfo, runtimeSettings, availabilityHandler);

        value(presenter);
    }
}

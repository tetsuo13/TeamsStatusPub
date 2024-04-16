using System;
using Microsoft.Extensions.Options;
using NSubstitute;
using TeamsStatusPub.Core.Models;
using TeamsStatusPub.Core.Presenters;
using TeamsStatusPub.Core.Services;
using TeamsStatusPub.Core.Services.AvailabilityHandlers;
using Xunit;

namespace TeamsStatusPub.Core.UnitTests.Presenters;

public static class AboutFormPresenterTests
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

        var presenter = new AboutFormPresenter(appInfo, runtimeSettings, availabilityHandler);

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

        var presenter = new AboutFormPresenter(appInfo, runtimeSettings, availabilityHandler);

        Assert.Equal($"http://{runtimeSettings.Value.ListenAddress}:{runtimeSettings.Value.ListenPort}/",
            presenter.ListenUrl);
    }

    [Fact]
    public static void ApplicationName_FromAppInfo()
    {
        var expected = "2023 Acme Corp, LLC";
        var appInfo = Substitute.For<IAppInfo>();
        appInfo.ApplicationName.Returns(expected);

        TestPropertyFromAppInfo(appInfo,
            (IAboutFormPresenter presenter) => Assert.Equal(expected, presenter.ApplicationName));
    }

    [Fact]
    public static void Copright_FromAppInfo()
    {
        var expected = "2023 Acme Corp, LLC";
        var appInfo = Substitute.For<IAppInfo>();
        appInfo.Copyright.Returns(expected);

        TestPropertyFromAppInfo(appInfo,
            (IAboutFormPresenter presenter) => Assert.Equal(expected, presenter.Copyright));
    }

    [Fact]
    public static void WebsiteUrl_FromAppInfo()
    {
        var expected = "https://youtu.be/dQw4w9WgXcQ";
        var appInfo = Substitute.For<IAppInfo>();
        appInfo.WebsiteUrl.Returns(expected);

        TestPropertyFromAppInfo(appInfo,
            (IAboutFormPresenter presenter) => Assert.Equal(expected, presenter.WebsiteUrl));
    }

    [Fact]
    public static void Version_FromAppInfo()
    {
        var expected = "1.2.3";
        var appInfo = Substitute.For<IAppInfo>();
        appInfo.Version.Returns(expected);

        TestPropertyFromAppInfo(appInfo,
            (IAboutFormPresenter presenter) => Assert.Equal(expected, presenter.Version));
    }

    private static void TestPropertyFromAppInfo(IAppInfo appInfo, Action<IAboutFormPresenter> value)
    {
        var runtimeSettings = Options.Create(new RuntimeSettings());
        var availabilityHandler = Substitute.For<IAvailabilityHandler>();

        var presenter = new AboutFormPresenter(appInfo, runtimeSettings, availabilityHandler);

        value(presenter);
    }
}

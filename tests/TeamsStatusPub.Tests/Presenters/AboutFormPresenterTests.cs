using System;
using Microsoft.Extensions.Options;
using Moq;
using TeamsStatusPub.Models;
using TeamsStatusPub.Presenters;
using TeamsStatusPub.Services;
using TeamsStatusPub.Services.AvailabilityHandlers;
using Xunit;

namespace TeamsStatusPub.Tests.Presenters;

public static class AboutFormPresenterTests
{
    [Theory]
    [InlineData(true, "not busy")]
    [InlineData(false, "busy")]
    public static void LastTeamsStatus_IsAvailable_ReturnsBusyStatus(bool isAvailable, string expectedStatus)
    {
        var appInfo = new Mock<IAppInfo>();
        var runtimeSettings = Options.Create(new RuntimeSettings());
        var availabilityHandler = new Mock<IAvailabilityHandler>();
        availabilityHandler.Setup(x => x.IsAvailable()).Returns(isAvailable);

        var presenter = new AboutFormPresenter(appInfo.Object, runtimeSettings, availabilityHandler.Object);

        Assert.Equal(expectedStatus, presenter.LastTeamsStatus);
    }

    [Fact]
    public static void ListenUrl_RuntimeSettings_ShowsListenAddressAndPort()
    {
        var appInfo = new Mock<IAppInfo>();
        var runtimeSettings = Options.Create(new RuntimeSettings
        {
            ListenAddress = "10.11.12.13",
            ListenPort = 12345
        });
        var availabilityHandler = new Mock<IAvailabilityHandler>();

        var presenter = new AboutFormPresenter(appInfo.Object, runtimeSettings, availabilityHandler.Object);

        Assert.Equal($"http://{runtimeSettings.Value.ListenAddress}:{runtimeSettings.Value.ListenPort}/",
            presenter.ListenUrl);
    }

    [Fact]
    public static void ApplicationName_FromAppInfo()
    {
        var expected = "2023 Acme Corp, LLC";
        var appInfo = new Mock<IAppInfo>();
        appInfo.Setup(x => x.ApplicationName).Returns(expected);

        TestPropertyFromAppInfo(appInfo,
            (IAboutFormPresenter presenter) => Assert.Equal(expected, presenter.ApplicationName));
    }

    [Fact]
    public static void Copright_FromAppInfo()
    {
        var expected = "2023 Acme Corp, LLC";
        var appInfo = new Mock<IAppInfo>();
        appInfo.Setup(x => x.Copyright).Returns(expected);

        TestPropertyFromAppInfo(appInfo,
            (IAboutFormPresenter presenter) => Assert.Equal(expected, presenter.Copyright));
    }

    [Fact]
    public static void WebsiteUrl_FromAppInfo()
    {
        var expected = "https://youtu.be/dQw4w9WgXcQ";
        var appInfo = new Mock<IAppInfo>();
        appInfo.Setup(x => x.WebsiteUrl).Returns(expected);

        TestPropertyFromAppInfo(appInfo,
            (IAboutFormPresenter presenter) => Assert.Equal(expected, presenter.WebsiteUrl));
    }

    [Fact]
    public static void Version_FromAppInfo()
    {
        var expected = "1.2.3";
        var appInfo = new Mock<IAppInfo>();
        appInfo.Setup(x => x.Version).Returns(expected);

        TestPropertyFromAppInfo(appInfo,
            (IAboutFormPresenter presenter) => Assert.Equal(expected, presenter.Version));
    }

    private static void TestPropertyFromAppInfo(Mock<IAppInfo> appInfo,
        Action<IAboutFormPresenter> value)
    {
        var runtimeSettings = Options.Create(new RuntimeSettings());
        var availabilityHandler = new Mock<IAvailabilityHandler>();

        var presenter = new AboutFormPresenter(appInfo.Object, runtimeSettings, availabilityHandler.Object);

        value(presenter);
    }
}

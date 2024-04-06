using System.Collections.Generic;
using TeamsStatusPub.Core.Services.AvailabilityHandlers;
using Xunit;

namespace TeamsStatusPub.Core.UnitTests.Services.AvailabilityHandlers.MicrosoftTeamsClassic;

public static class MicrosoftTeamsClassicHandlerTests
{
    private const string EventData0 = "Tue Aug 16 2022 10:50:13 GMT-0400 (Eastern Daylight Time) <159540> -- event -- eventpdclevel: 2, eventData: s::;m::1;a::0, inactiveTime: 0, name: machineState, AppInfo.Language: en-us, complianceEnvironmentType: 0, isDataCategorizationEnabled: true, userpdclevel: 0, processMemory: 25340460, freeMemory: 3206307840, clientType: desktop, AppInfo.ClientType: desktop, batterylevel: 0.95, pluggedin: true, Window.Focus: foreground, windowIsVisible: true, Window.Status: maximized, UserInfo.TimeZone: -04:00, vdiMode: 0,";
    private const string EventData1 = "Wed Mar 30 2022 11:30:31 GMT-0400 (Eastern Daylight Time) <25392> -- event -- eventpdclevel: 2, eventData: s::;m::1;a::1, inactiveTime: 1, name: machineState, AppInfo.Language: en-US, complianceEnvironmentType: 0, isDataCategorizationEnabled: true, userpdclevel: 0, processMemory: 27548024, freeMemory: 344715264, clientType: desktop, AppInfo.ClientType: desktop, batterylevel: 1, pluggedin: true, Window.Focus: foreground, windowIsVisible: true, Window.Status: maximized, UserInfo.TimeZone: -04:00, vdiMode: 0,";
    private const string EventData3 = "Wed Mar 30 2022 12:30:14 GMT-0400 (Eastern Daylight Time) <25392> -- event -- eventpdclevel: 2, eventData: s::;m::1;a::3, inactiveTime: 0, name: machineState, AppInfo.Language: en-US, complianceEnvironmentType: 0, isDataCategorizationEnabled: true, userpdclevel: 0, processMemory: 28988980, freeMemory: 402382848, clientType: desktop, AppInfo.ClientType: desktop, batterylevel: 1, pluggedin: true, Window.Focus: foreground, windowIsVisible: true, Window.Status: maximized, UserInfo.TimeZone: -04:00, vdiMode: 0,";
    private const string EventData4 = "Wed Mar 30 2022 16:23:16 GMT-0400 (Eastern Daylight Time) <14080> -- event -- eventpdclevel: 2, eventData: s::;m::1;a::4, inactiveTime: 42, name: machineState, AppInfo.Language: en-us, complianceEnvironmentType: 0, isDataCategorizationEnabled: true, userpdclevel: 0, processMemory: 24242768, freeMemory: 6676017152, clientType: desktop, AppInfo.ClientType: desktop, batterylevel: 0.99, pluggedin: true, Window.Focus: foreground, windowIsVisible: true, Window.Status: maximized, UserInfo.TimeZone: -04:00, vdiMode: 0,";
    private const string EventData5 = "Wed Mar 30 2022 21:52:24 GMT-0400 (Eastern Daylight Time) <25392> -- event -- eventpdclevel: 2, eventData: s::;m::1;a::5, inactiveTime: 60, name: machineState, AppInfo.Language: en-US, complianceEnvironmentType: 0, isDataCategorizationEnabled: true, userpdclevel: 0, processMemory: 26274384, freeMemory: 992256000, clientType: desktop, AppInfo.ClientType: desktop, batterylevel: 1, pluggedin: true, Window.Focus: foreground, windowIsVisible: true, Window.Status: maximized, UserInfo.TimeZone: -04:00, vdiMode: 0,";

    [Fact]
    public static void LastAvailabilityFromLinesOfInterest_NoLines_ReturnsNull()
    {
        Assert.Null(MicrosoftTeamsClassicHandler.LastAvailabilityFromLinesOfInterest([]));
    }

    [Fact]
    public static void LastAvailabilityFromLinesOfInterest_NoEventToken_ReturnsTrue()
    {
        var lines = new List<string>
        {
            "",
            "Tue Apr 05 2022 08:39:03 GMT-0400 (Eastern Daylight Time) <14080> -- info -- Focusing window 1",
            "Tue Apr 05 2022 08:48:37 GMT-0400 (Eastern Daylight Time) <14080> -- event -- eventpdclevel: 1, errorMessage: https://: line 23: CommandReportingServiceHybrid: appendCommandEventData called with correlationId 185c2d1a-0dc5-4519-94de-b65c3e51e0fd, but there was no such pending command, status: success, scenario: 8f387670-9490-4827-af02-155f2872e0e4, scenarioName: experience-renderer-console-error, name: experience-renderer-console-error, step: start, sequence: 0, delta: 0, scenarioDelta: 0, elapsed: 410415997, stepDelta: 0, Scenario.Mode: 1, AppInfo.Language: en-us, complianceEnvironmentType: 0, isDataCategorizationEnabled: true, userpdclevel: 0, processMemory: 28156884, freeMemory: 2940518400, clientType: desktop, AppInfo.ClientType: desktop, batterylevel: 1, pluggedin: true, Window.Focus: foreground, windowIsVisible: true, Window.Status: maximized, UserInfo.TimeZone: -04:00, vdiMode: 0,",
            @"Tue Apr 05 2022 09:29:17 GMT-0400 (Eastern Daylight Time) <28980> -- event -- eventpdclevel: 2, name: desktop_current_configuration, eventData: {""preventUpnDetectSso"":false,""currentWebLanguage"":""en-US"",""silentUpdateTriggered"":false,""rendererCrashInfo"":null,""previousCrashesInfo"":{""crashes"":[{""datetime"":1633392258929,""type"":""renderer"",""isPreviousCrash"":true},{""datetime"":1633342561876,""type"":""renderer"",""isPreviousCrash"":true},{""datetime"":1633136252111,""type"":""renderer"",""isPreviousCrash"":true}]},""windowState"":{""monitorId"":2528732444,""x"":2352,""y"":50,""width"":1488,""height"":788,""isMaximized"":true,""isFullScreen"":false},""surfaceHubWindowState"":{""monitorId"":-1,""x"":0,""y"":0,""width"":400,""height"":600,""isMaximized"":true,""isFullScreen"":false},""featureLaunchInfo"":{""authWamAccountEnumerationAppStartTelemetry"":-1},""restartCommand"":{},""launchTime"":""1648752501743"",""isLoggedOut"":false,""appPreferenceSettings"":{""disableGpu"":false,""openAtLogin"":true,""openAsHidden"":false,""runningOnClose"":true,""registerAsIMProvider"":true},""isAppFirstRun"":false,""isAppSessionEnd"":false,""isAppTerminated"":false,""isForeground"":false,""machineId"":""0071894b0367f6cc81ded16db7f0da27d39ec004306c524aa3110990ccc293fc"",""deviceInfoId"":""6f017743877c48bdcec6b3ee3660adb3f2362050b8274581c40985ff061d2609"",""restartInfo"":null,""notificationWindowOnClose"":true,""desktopSessionId"":""desktop - 7484aa53 - b8e3 - 417d - b590 - d2ec0fb883f0"",""upnScreenShowCount"":0}, AppInfo.Language: undefined, complianceEnvironmentType: 0, isDataCategorizationEnabled: true, userpdclevel: 0, processMemory: 21942576, freeMemory: 4403195904, clientType: desktop, AppInfo.ClientType: desktop, UserInfo.TimeZone: -04:00, vdiMode: false,"
        };

        Assert.True(MicrosoftTeamsClassicHandler.LastAvailabilityFromLinesOfInterest(lines));
    }

    [Fact]
    public static void LastAvailabilityFromLinesOfInterest_CallStart_ReturnsFalse()
    {
        var lines = new List<string>
        {
            EventData1
        };

        Assert.False(MicrosoftTeamsClassicHandler.LastAvailabilityFromLinesOfInterest(lines));
    }

    [Fact]
    public static void LastAvailabilityFromLinesOfInterest_CallStartCallStop_ReturnsTrue()
    {
        var lines = new List<string>
        {
            EventData3,
            EventData1
        };

        Assert.True(MicrosoftTeamsClassicHandler.LastAvailabilityFromLinesOfInterest(lines));
    }

    [Fact]
    public static void LastAvailabilityFromLinesOfInterest_CallStartScreenShareStart_ReturnsFalse()
    {
        var lines = new List<string>
        {
            EventData0,
            EventData1
        };

        Assert.False(MicrosoftTeamsClassicHandler.LastAvailabilityFromLinesOfInterest(lines));
    }

    [Fact]
    public static void LastAvailabilityFromLinesOfInterest_CallStartScreenShareStartScreenShareStop_ReturnsFalse()
    {
        var lines = new List<string>
        {
            EventData1,
            EventData0,
            EventData1
        };

        Assert.False(MicrosoftTeamsClassicHandler.LastAvailabilityFromLinesOfInterest(lines));
    }

    [Fact]
    public static void LastAvailabilityFromLinesOfInterest_CallStartScreenShareStartScreenShareStopCallStop_ReturnsTrue()
    {
        var lines = new List<string>
        {
            EventData3,
            EventData1,
            EventData0,
            EventData1
        };

        Assert.True(MicrosoftTeamsClassicHandler.LastAvailabilityFromLinesOfInterest(lines));
    }

    [Fact]
    public static void LastAvailabilityFromLinesOfInterest_NoCallStartedEventToken_ReturnsFalse()
    {
        var lines = new List<string>
        {
            EventData3,
            EventData4,
            EventData5
        };

        Assert.True(MicrosoftTeamsClassicHandler.LastAvailabilityFromLinesOfInterest(lines));
    }
}

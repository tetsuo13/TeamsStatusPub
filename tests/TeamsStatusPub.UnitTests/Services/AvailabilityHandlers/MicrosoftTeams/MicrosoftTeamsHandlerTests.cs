using Microsoft.Extensions.Logging;
using NSubstitute;
using TeamsStatusPub.Services.AvailabilityHandlers;
using TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams;
using TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams.FileSystemProviders;
using Xunit;

namespace TeamsStatusPub.UnitTests.Services.AvailabilityHandlers.MicrosoftTeams;

public class MicrosoftTeamsHandlerTests
{
    private const string EventData1 = """2024-04-01T08:38:46.492348-04:00 0x000041dc <INFO> native_modules::UserDataCrossCloudModule: BroadcastGlobalState: New Global State Event: UserDataGlobalState total number of users: 1 { user id :118c7aa5-4e0a-4276-9cd5-68e8e9ea9ede, availability: Available, unread notification count: 9 }""";
    private const string EventData2 = """2024-04-01T08:38:46.492348-04:00 0x000041dc <INFO> native_modules::UserDataCrossCloudModule: BroadcastGlobalState: New Global State Event: UserDataGlobalState total number of users: 1 { user id :118c7aa5-4e0a-4276-9cd5-68e8e9ea9ede, availability: Busy, unread notification count: 10 }""";
    private const string EventData3 = """2024-04-01T08:38:46.492348-04:00 0x000041dc <INFO> native_modules::UserDataCrossCloudModule: BroadcastGlobalState: New Global State Event: UserDataGlobalState total number of users: 1 { user id :118c7aa5-4e0a-4276-9cd5-68e8e9ea9ede, availability: Away, unread notification count: 8 }""";
    private const string EventData4 = """2024-04-01T08:38:46.492348-04:00 0x000041dc <INFO> native_modules::UserDataCrossCloudModule: BroadcastGlobalState: New Global State Event: UserDataGlobalState total number of users: 1 { user id :118c7aa5-4e0a-4276-9cd5-68e8e9ea9ede, availability: DoNotDisturb, unread notification count: 8 }""";
    private const string EventData5 = """2024-04-01T08:38:46.492348-04:00 0x000041dc <INFO> native_modules::UserDataCrossCloudModule: BroadcastGlobalState: New Global State Event: UserDataGlobalState total number of users: 1 { user id :118c7aa5-4e0a-4276-9cd5-68e8e9ea9ede, availability: BeRightBack, unread notification count: 8 }""";

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsAvailable_MissingLogDirectory_ReturnsLastAvailable(bool lastAvailable)
    {
        _logDiscovery.FindLogDirectory().Returns((string?)null);
        var handler = GetHandler(lastAvailable);

        var actual = handler.IsAvailable();

        Assert.Equal(lastAvailable, actual);
        _logDiscovery.DidNotReceiveWithAnyArgs().FindLogPath(Arg.Any<string>());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsAvailable_MissingLogFile_ReturnsLastAvailable(bool lastAvailable)
    {
        _logDiscovery.FindLogDirectory().Returns(nameof(IsAvailable_MissingLogFile_ReturnsLastAvailable));
        _logDiscovery.FindLogPath(Arg.Any<string>()).Returns((string?)null);
        var handler = GetHandler(lastAvailable);

        var actual = handler.IsAvailable();

        Assert.Equal(lastAvailable, actual);
        _fileSystemProvider.DidNotReceiveWithAnyArgs().ReadAllLines(Arg.Any<string>());
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsAvailable_NoEventData_ReturnsLastAvailable(bool lastAvailable)
    {
        _fileSystemProvider.ReadAllLines(Arg.Any<string>()).Returns([]);

        _logDiscovery.FindLogDirectory().Returns(nameof(IsAvailable_NoEventData_ReturnsLastAvailable));
        _logDiscovery.FindLogPath(Arg.Any<string>()).Returns(nameof(IsAvailable_NoEventData_ReturnsLastAvailable));
        var handler = GetHandler(lastAvailable);

        var actual = handler.IsAvailable();

        Assert.Equal(lastAvailable, actual);
    }

    public static TheoryData<string, bool> AvailabilityLogLines => new()
    {
        { EventData1, true },
        { EventData2, false },
        { EventData3, true },
        { EventData4, false },
        { EventData5, true }
    };

    [Theory]
    [MemberData(nameof(AvailabilityLogLines))]
    public void IsAvailable_ParseEventData(string line, bool expected)
    {
        _fileSystemProvider.ReadAllLines(Arg.Any<string>()).Returns([line]);

        _logDiscovery.FindLogDirectory().Returns(nameof(IsAvailable_ParseEventData));
        _logDiscovery.FindLogPath(Arg.Any<string>()).Returns(nameof(IsAvailable_ParseEventData));
        var handler = GetHandler();

        var actual = handler.IsAvailable();

        Assert.Equal(expected, actual);
    }

    private readonly IFileSystemProvider _fileSystemProvider = Substitute.For<IFileSystemProvider>();
    private readonly ILogDiscovery _logDiscovery = Substitute.For<ILogDiscovery>();

    private MicrosoftTeamsHandler GetHandler(bool lastAvailable = true)
    {
        var logger = Substitute.For<ILogger<MicrosoftTeamsHandler>>();
        return new MicrosoftTeamsHandler(logger, _fileSystemProvider, _logDiscovery, lastAvailable);
    }
}

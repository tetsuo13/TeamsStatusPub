using Microsoft.Extensions.Logging;
using NSubstitute;
using TeamsStatusPub.Services.AvailabilityHandlers;
using TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams;
using TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams.FileSystemProviders;
using Xunit;

namespace TeamsStatusPub.UnitTests.Services.AvailabilityHandlers.MicrosoftTeams;

public class MicrosoftTeamsHandlerTests
{
    private const string EventData1 = """2024-03-01T14:19:16.252320-04:00 0x00005038 <DBG>  TaskbarBadgeServiceLegacy:Work: SetBadge Setting badge: GlyphBadge{"available"}, overlay: No items, status available""";
    private const string EventData2 = """2024-03-02T14:19:16.252320-04:00 0x00005038 <DBG>  TaskbarBadgeServiceLegacy:Work: SetBadge Setting badge: GlyphBadge{"away"}, overlay: No items, status away""";
    private const string EventData3 = """2024-03-03T14:19:16.252320-04:00 0x00005038 <DBG>  TaskbarBadgeServiceLegacy:Work: SetBadge Setting badge: GlyphBadge{"busy"}, overlay: No items, status busy""";
    private const string EventData4 = """2024-03-04T14:19:16.252320-04:00 0x00005038 <DBG>  TaskbarBadgeServiceLegacy:Work: SetBadge Setting badge: GlyphBadge{"offline"}, overlay: No items, status offline""";
    private const string EventData5 = """2024-03-05T14:19:16.252320-04:00 0x00005038 <DBG>  TaskbarBadgeServiceLegacy:Work: SetBadge Setting badge: GlyphBadge{"doNotDistrb"}, overlay: No items, status doNotDisturb""";
    private const string EventData6 = """2024-03-06T14:19:16.252320-04:00 0x00005038 <DBG>  TaskbarBadgeServiceLegacy:Work: SetBadge Setting badge: GlyphBadge{"available"}, overlay: アイテムなし、状態 available""";

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
        { EventData2, true },
        { EventData3, false },
        { EventData4, true },
        { EventData5, false },
        { EventData6, true }
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

using Microsoft.Extensions.Logging;
using NSubstitute;
using TeamsStatusPub.Core.Services.AvailabilityHandlers.MicrosoftTeams;
using TeamsStatusPub.Core.Services.AvailabilityHandlers.MicrosoftTeams.FileSystemProviders;
using Xunit;

namespace TeamsStatusPub.Core.UnitTests.Services.AvailabilityHandlers.MicrosoftTeams;

public class LogDiscoveryTests
{
    [Fact]
    public void FindLogDirectory_MissingPackagesDirectory_ReturnsNull()
    {
        _fileSystemProvider.Directory.Exists(default).ReturnsForAnyArgs(false);
        var logDiscovery = GetLogDiscovery();

        var actual = logDiscovery.FindLogDirectory();

        Assert.Null(actual);
        _fileSystemProvider.Directory.DidNotReceiveWithAnyArgs().GetDirectories(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public void FindLogDirectory_NoMSTeamsDirectories_ReturnsNull()
    {
        _fileSystemProvider.Directory.Exists(default).ReturnsForAnyArgs(true);
        _fileSystemProvider.Directory.GetDirectories(Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs([]);
        var logDiscovery = GetLogDiscovery();

        var actual = logDiscovery.FindLogDirectory();

        Assert.Null(actual);
        _fileSystemProvider.Directory.Received(1).GetDirectories(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public void FindLogDirectory_MoreThanOneMSTeamsDirectory_ReturnsNull()
    {
        _fileSystemProvider.Directory.Exists(default).ReturnsForAnyArgs(true);
        _fileSystemProvider.Directory.GetDirectories(Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs(["MSTeams_3ahsuwkd5mfie", "MSTeams_9fjduh2bbsjal"]);
        var logDiscovery = GetLogDiscovery();

        var actual = logDiscovery.FindLogDirectory();

        Assert.Null(actual);
        _fileSystemProvider.Directory.Received(1).GetDirectories(Arg.Any<string>(), Arg.Any<string>());
    }

    [Fact]
    public void FindLogDirectory_MissingLogsDirectory_ReturnsNull()
    {
        _fileSystemProvider.Directory.Exists(default).ReturnsForAnyArgs(
            // Check for Packages directory
            x => true,
            // Check for Logs directory
            x => false);
        _fileSystemProvider.Directory.GetDirectories(Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs(["MSTeams_3ahsuwkd5mfie"]);
        var logDiscovery = GetLogDiscovery();

        var actual = logDiscovery.FindLogDirectory();

        Assert.Null(actual);
        _fileSystemProvider.Directory.Received(2).Exists(Arg.Any<string>());
    }

    [Fact]
    public void FindLogDirectory_LogsDirectoryExists_ReturnsPath()
    {
        _fileSystemProvider.Directory.Exists(default).ReturnsForAnyArgs(true);
        _fileSystemProvider.Directory.GetDirectories(Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs(["MSTeams_3ahsuwkd5mfie"]);
        var logDiscovery = GetLogDiscovery();

        var actual = logDiscovery.FindLogDirectory();

        Assert.EndsWith("Logs", actual);
        _fileSystemProvider.Directory.Received(2).Exists(Arg.Any<string>());
    }

    [Fact]
    public void FindLogPath_NoLogsFiles_ReturnsNull()
    {
        _fileSystemProvider.Directory.GetFiles(Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs([]);
        var logDiscovery = GetLogDiscovery();

        var actual = logDiscovery.FindLogPath(nameof(FindLogPath_NoLogsFiles_ReturnsNull));

        Assert.Null(actual);
    }

    [Fact]
    public void FindLogPath_LogsFiles_ReturnsMostRecent()
    {
        const string expected = "MSTeams_2024-03-25_08-13-12.02.log";
        string[] logsFiles = [
            "MSTeams_2024-03-25_08-13-12.01.log",
            expected,
            "MSTeams_2024-03-25_08-13-12.01.log"
            ];
        _fileSystemProvider.Directory.GetFiles(Arg.Any<string>(), Arg.Any<string>()).ReturnsForAnyArgs(logsFiles);
        var logDiscovery = GetLogDiscovery();

        var actual = logDiscovery.FindLogPath(nameof(FindLogPath_NoLogsFiles_ReturnsNull));

        Assert.Equal(expected, actual);
    }

    private readonly IFileSystemProvider _fileSystemProvider = Substitute.For<IFileSystemProvider>();

    private LogDiscovery GetLogDiscovery()
    {
        var logger = Substitute.For<ILogger<LogDiscovery>>();
        return new LogDiscovery(logger, _fileSystemProvider);
    }
}

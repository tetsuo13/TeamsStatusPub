using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeamsClassic;
using Xunit;

namespace TeamsStatusPub.IntegrationTests.Services.AvailabilityHandlers.MicrosoftTeamsClassic;

public static class LogFileReaderTests
{
    /// <summary>
    /// Doesn't contain a line with the event data token.
    /// </summary>
    private const string LogFile1 = "log1.txt";

    /// <summary>
    /// Has more lines than is configured to read.
    /// </summary>
    private const string LogFile2 = "log2.txt";

    /// <summary>
    /// Start and end of a call.
    /// </summary>
    private const string LogFile3 = "log3.txt";

    /// <summary>
    /// Start of a call.
    /// </summary>
    private const string LogFile4 = "log4.txt";

    /// <summary>
    /// Call started, screen share start, then end of call.
    /// </summary>
    private const string LogFile5 = "log5.txt";

    /// <summary>
    /// Call started and then the start of screen share.
    /// </summary>
    private const string LogFile6 = "log6.txt";

    /// <summary>
    /// Call started, start of screen share, then end of screen share.
    /// </summary>
    private const string LogFile7 = "log7.txt";

    /// <summary>
    /// Call started, start of screen share, end of screen share, end of call.
    /// </summary>
    private const string LogFile8 = "log8.txt";

    [Fact]
    public static void FileDoesntExist_ThrowsException()
    {
        var reader = GetReader();
        Assert.Throws<FileNotFoundException>(() => reader.ReadLinesOfInterest("ThisFileDefinitelyDoesNotExist42.txt"));
    }

    [Fact]
    public static void FileEmpty_ReturnsEmpty()
    {
        var logFile = Path.GetTempFileName();
        var actual = GetReader().ReadLinesOfInterest(GetLogFilePath(logFile));

        Assert.Empty(actual);
        Assert.DoesNotContain(actual, LogFileReader.ContainsEventToken);

        try
        {
            File.Delete(logFile);
        }
        catch
        {
            // Assume system cleanup will eventually delete it.
        }
    }

    [Theory]
    [InlineData(LogFile1)]
    [InlineData(LogFile2)]
    public static void NoEventDataLines(string logFile)
    {
        var actual = GetReader().ReadLinesOfInterest(GetLogFilePath(logFile));

        Assert.Empty(actual);
        Assert.DoesNotContain(actual, LogFileReader.ContainsEventToken);
    }

    [Fact]
    public static void CallStart_CallStop()
    {
        var actual = GetReader().ReadLinesOfInterest(GetLogFilePath(LogFile3));
        RunAssertions(actual, 3);
    }

    [Fact]
    public static void CallStart_ScreenShareStart()
    {
        var actual = GetReader().ReadLinesOfInterest(GetLogFilePath(LogFile6));
        RunAssertions(actual, 1);
    }

    [Fact]
    public static void CallStart_ScreenShareStart_ScreenShareStop()
    {
        var actual = GetReader().ReadLinesOfInterest(GetLogFilePath(LogFile7));
        RunAssertions(actual, 2);
    }

    [Fact]
    public static void CallStart_ScreenShareStart_ScreenShareStop_CallStop()
    {
        var actual = GetReader().ReadLinesOfInterest(GetLogFilePath(LogFile8));
        RunAssertions(actual, 3);
    }

    [Fact]
    public static void CallStart_ScreenShareStart_CallStop()
    {
        var actual = GetReader().ReadLinesOfInterest(GetLogFilePath(LogFile5));
        RunAssertions(actual, 2);
    }

    [Fact]
    public static void CallStart()
    {
        var actual = GetReader().ReadLinesOfInterest(GetLogFilePath(LogFile4));
        RunAssertions(actual, 1);
    }

    private static string GetLogFilePath(string fileName)
    {
        var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        Assert.NotNull(assemblyDirectory);

        var logDirectory = Path.Combine(nameof(Services), nameof(AvailabilityHandlers), nameof(MicrosoftTeamsClassic), "ExampleLogs");
        var logfilePath = Path.Combine(assemblyDirectory, logDirectory, fileName);

        Assert.True(File.Exists(logfilePath),
            $"Couldn't find log file {fileName} under assembly location '{assemblyDirectory}' subdirectory '{logDirectory}'");

        return logfilePath;
    }

    private static void RunAssertions(List<string> linesOfInterest, int expectedCount)
    {
        Assert.NotEmpty(linesOfInterest);
        Assert.Equal(expectedCount, linesOfInterest.Count);
        Assert.True(linesOfInterest.TrueForAll(LogFileReader.ContainsEventToken));
    }

    private static LogFileReader GetReader()
    {
        var logger = Substitute.For<ILogger<LogFileReader>>();
        return new LogFileReader(logger);
    }
}

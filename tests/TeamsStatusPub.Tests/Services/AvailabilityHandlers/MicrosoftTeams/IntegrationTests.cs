using System.IO;
using System.Reflection;
using Xunit;

namespace TeamsStatusPub.Tests.Services.AvailabilityHandlers.MicrosoftTeams;

public class IntegrationTests : MicrosoftTeamsHandlerTests
{
    /// <summary>
    /// Log file that doesn't contain a line with the event data token.
    /// </summary>
    private const string NoEventDataTokenLogFile = "log1.txt";

    /// <summary>
    /// Log file that contains more lines than is configured to read.
    /// </summary>
    private const string TooManyLinesOfNothingLogFile = "log2.txt";

    /// <summary>
    /// Log file that contains the start and end of a call.
    /// </summary>
    private const string StartedThenEndedCallLogFile = "log3.txt";

    /// <summary>
    /// Log file that contains the start of a call.
    /// </summary>
    private const string StartedCallLogFile = "log4.txt";

    [Fact]
    public static void FindLastAvailabilityFromLogFile_FileDoesntExist_ThrowsException()
    {
        var handler = GetHandler();
        Assert.Throws<FileNotFoundException>(() => handler.FindLastAvailabilityFromLogFile("ThisFileDefinitelyDoesNotExist42.txt"));
    }

    public static readonly TheoryData<string> _noEventTokenData = new()
    {
        NoEventDataTokenLogFile,
        TooManyLinesOfNothingLogFile
    };

    public static readonly TheoryData<string> _isAvailableData = new()
    {
        StartedThenEndedCallLogFile
    };

    public static readonly TheoryData<string> _isNotAvailableData = new()
    {
        StartedCallLogFile
    };

    [Theory]
    [MemberData(nameof(_noEventTokenData))]
    public static void FindLastAvailabilityFromLogFile_NeverFindsEventDataLine_ReturnsNull(string logFile)
    {
        var handler = GetHandler();
        var actual = handler.FindLastAvailabilityFromLogFile(GetLogFilePath(logFile));

        Assert.Null(actual);
    }

    /// <summary>
    /// Log file contains one or more event data lines however the last
    /// instance indicates that the user is available.
    /// </summary>
    [Theory]
    [MemberData(nameof(_isAvailableData))]
    public static void FindLastAvailabilityFromLogFile_ContainsEventData_ReturnsTrue(string logFile)
    {
        var handler = GetHandler();
        var actual = handler.FindLastAvailabilityFromLogFile(GetLogFilePath(logFile));

        Assert.NotNull(actual);
        Assert.True(actual.HasValue);
        Assert.True(actual.Value);
    }

    /// <summary>
    /// Log file contains one or more event data lines however the last
    /// instance indicates that the user isn't available.
    /// </summary>
    [Theory]
    [MemberData(nameof(_isNotAvailableData))]
    public static void FindLastAvailabilityFromLogFile_ContainsEventData_ReturnsFalse(string logFile)
    {
        var handler = GetHandler();
        var actual = handler.FindLastAvailabilityFromLogFile(GetLogFilePath(logFile));

        Assert.NotNull(actual);
        Assert.True(actual.HasValue);
        Assert.False(actual.Value);
    }

    private static string GetLogFilePath(string fileName)
    {
        var assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        Assert.NotNull(assemblyDirectory);

        var logDirectory = Path.Combine(nameof(Services), nameof(AvailabilityHandlers), nameof(MicrosoftTeams), "ExampleLogs");
        var logfilePath = Path.Combine(assemblyDirectory, logDirectory, fileName);

        Assert.True(File.Exists(logfilePath),
            $"Couldn't find log file {fileName} under assembly location '{assemblyDirectory}' subdirectory '{logDirectory}'");

        return logfilePath;
    }
}

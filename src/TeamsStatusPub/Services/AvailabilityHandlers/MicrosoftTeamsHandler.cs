using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

namespace TeamsStatusPub.Services.AvailabilityHandlers;

/// <summary>
/// Availability of user in the Microsoft Teams desktop application. This
/// doesn't handle the mobile or web versions.
/// </summary>
/// <remarks>
/// Reads the log file that Teams creates at <see cref="_teamsLogFilePath"/> to
/// find tokens that indicate that a call was started or has since stopped. If
/// the <see cref="_eventDataTokenCallStarted"/> token is found on a line then
/// it indicates that a call was started. Any line found that contains a token
/// that begins with <see cref="EventDataToken"/> (but not
/// <see cref="_eventDataTokenCallStarted"/>) afterwards indicates that the
/// call has ended.
/// </remarks>
public class MicrosoftTeamsHandler : IAvailabilityHandler
{
    /// <summary>
    /// The result of the last log file processing.
    /// </summary>
    private bool _lastAvailability = true;

    /// <summary>
    /// The base event data token. A number concatenated with this token
    /// indicates a certain event has occurred, the most significant being
    /// <see cref="_eventDataTokenCallStarted"/>.
    /// </summary>
    private const string EventDataToken = "eventData: s::;m::1;a::";

    /// <summary>
    /// The absolute path to the Teams log file.
    /// </summary>
    private readonly string _teamsLogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Microsoft", "Teams", "logs.txt");

    /// <summary>
    /// A line with this token indicates that a call has been started.
    /// </summary>
    private readonly string _eventDataTokenCallStarted = EventDataToken + "1";

    private readonly ILogger<MicrosoftTeamsHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the MicrosoftTeamsHandler class.
    /// </summary>
    /// <param name="logger"></param>
    public MicrosoftTeamsHandler(ILogger<MicrosoftTeamsHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc/>
    /// <remarks>
    /// File is read backwards (reverse chronological order) to find the first
    /// event data token.
    /// </remarks>
    public bool IsAvailable()
    {
        if (!File.Exists(_teamsLogFilePath))
        {
            _logger.LogCritical("Couldn't find Teams log file at {logFilePath}", _teamsLogFilePath);
            throw new FileNotFoundException("Couldn't find Teams log file", _teamsLogFilePath);
        }

        Stopwatch? stopwatch = null;

        var lineBuilder = new List<char>();
        var linesRead = 0;

        // Stop counter. The event data tokens are sprinkled often in the log
        // file unless a call is active in which case there may be thousands
        // of lines added until the next event token is created when the call
        // is completed. No need to read the lines unnecessarily.
        const int maxLines = 500;

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            stopwatch = new Stopwatch();
            stopwatch.Start();
        }

        using var stream = new FileStream(_teamsLogFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        stream.Seek(0, SeekOrigin.End);

        while (true)
        {
            var bufferLength = 1;
            var buffer = new byte[1];
            // Assumes multi-byte UTF-8 file encoding.
            const char invalidChar = '�'; // 65533
            char[] chars = { invalidChar };
            var iteration = 0;

            // Read a single multi-byte character.
            while (chars.Contains(invalidChar))
            {
                if (iteration > 0)
                {
                    bufferLength = buffer.Length + 1;
                    var newBuffer = new byte[bufferLength];
                    Array.Copy(buffer, newBuffer, bufferLength - 1);
                    buffer = newBuffer;
                }

                // UTF-8 is 4 bytes per character.
                if (iteration > 4)
                {
                    throw new Exception();
                }

                try
                {
                    stream.Seek(-bufferLength, SeekOrigin.Current);
                }
                catch
                {
                    // Reached the beginning of the file.
                    chars = new char[] { '\0' };
                    break;
                }

                stream.Read(buffer, 0, bufferLength);
                var charCount = Encoding.UTF8.GetCharCount(buffer, 0, bufferLength);
                chars = new char[charCount];
                Encoding.UTF8.GetChars(buffer, 0, bufferLength, chars, 0);
                iteration++;
            }

            lineBuilder.InsertRange(0, chars);

            if (chars.Length > 0 && chars[0] == '\n')
            {
                var line = new string(lineBuilder.ToArray()).Trim();
                var containsCallStartedEventToken = ContainsCallStartedEventToken(line);

                if (containsCallStartedEventToken.HasValue)
                {
                    if (_logger.IsEnabled(LogLevel.Debug))
                    {
                        stopwatch?.Stop();
                        _logger.LogDebug("Read {linesRead} lines in {numSeconds} s", linesRead, stopwatch?.Elapsed.TotalSeconds);
                    }

                    // If the line contains the call started token then the
                    // user isn't available.
                    _lastAvailability = containsCallStartedEventToken.Value == false;
                    return _lastAvailability;
                }

                lineBuilder.Clear();
                linesRead++;
            }

            if (linesRead == maxLines)
            {
                break;
            }

            try
            {
                stream.Seek(-bufferLength, SeekOrigin.Current);
            }
            catch
            {
                break;
            }
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            stopwatch?.Stop();
            _logger.LogDebug("Couldn't find any event data tokens after reading {maxLines} lines in {numSeconds} s, returning last availability of {lastValue}",
                maxLines, stopwatch?.Elapsed.TotalSeconds, _lastAvailability);
        }

        return _lastAvailability;
    }

    /// <summary>
    /// Determines if a log file line contains the call started event data
    /// token.
    /// </summary>
    /// <param name="line">A single line from the log file.</param>
    /// <returns>
    /// Returns <see langword="null"/> if there were no event data tokens in
    /// the line; <see langword="true"/> if the
    /// <see cref="_eventDataTokenCallStarted"/> event token was found;
    /// otherwise <see langword="false"/>.
    /// </returns>
    internal bool? ContainsCallStartedEventToken(string line)
    {
        if (!line.Contains(EventDataToken))
        {
            return null;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Event data line: {line}", line);
        }

        return line.Contains(_eventDataTokenCallStarted);
    }
}

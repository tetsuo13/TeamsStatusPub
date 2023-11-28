using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

namespace TeamsStatusPub.Services.AvailabilityHandlers.MicrosoftTeams;

/// <summary>
/// Microsoft Teams log file reader. Designed to be instantiated each time the
/// log file needs to be read. Reads from the end of the file backwards until
/// it finds <see cref="MaxLinesOfInterestToRead"/> event data tokens as the
/// most recent log entries are at the end. Doesn't read file into memory as
/// file size may be large.
/// </summary>
public class LogFileReader : ILogFileReader
{
    /// <summary>
    /// Stop counter. The event data tokens are sprinkled often in the log
    /// file unless a call is active in which case there may be thousands of
    /// lines added until the next event token is created when the call is
    /// completed. No need to read the lines unnecessarily.
    /// </summary>
    private const int MaxLinesToRead = 500;

    /// <summary>
    /// Stop counter for lines of interest that are read.
    /// </summary>
    private const int MaxLinesOfInterestToRead = 3;

    private readonly ILogger<LogFileReader> _logger;
    private readonly List<string> _linesOfInterest = [];
    private readonly Stopwatch _stopwatch = new();

    private int _linesRead;
    private int _bufferLength;

    /// <summary>
    /// Initializes a new instance of the LogFileReader class.
    /// </summary>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public LogFileReader(ILogger<LogFileReader> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Read a single multi-byte character. Assumes UTF-8 encoding.
    /// </summary>
    /// <param name="stream"></param>
    /// <returns></returns>
    /// <exception cref="OverflowException">Thrown if more than 4 bytes are read.</exception>
    private char[] ReadCharacter(FileStream stream)
    {
        var buffer = new byte[1];
        const char invalidChar = '�'; // 65533
        char[] chars = [invalidChar];
        var iteration = 0;

        while (chars.Contains(invalidChar))
        {
            if (iteration > 0)
            {
                _bufferLength = buffer.Length + 1;
                var newBuffer = new byte[_bufferLength];
                Array.Copy(buffer, newBuffer, _bufferLength - 1);
                buffer = newBuffer;
            }

            // UTF-8 is 4 bytes per character.
            if (iteration > 4)
            {
                throw new OverflowException();
            }

            try
            {
                stream.Seek(-_bufferLength, SeekOrigin.Current);
            }
            catch
            {
                // Reached the beginning of the file.
                chars = ['\0'];
                break;
            }

            stream.Read(buffer, 0, _bufferLength);
            var charCount = Encoding.UTF8.GetCharCount(buffer, 0, _bufferLength);
            chars = new char[charCount];
            Encoding.UTF8.GetChars(buffer, 0, _bufferLength, chars, 0);
            iteration++;
        }

        return chars;
    }

    public List<string> ReadLinesOfInterest(string logFilePath)
    {
        _stopwatch.Start();
        var lineBuilder = new List<char>();

        using var stream = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        stream.Seek(0, SeekOrigin.End);

        while (true)
        {
            _bufferLength = 1;
            char[] chars = ReadCharacter(stream);

            lineBuilder.InsertRange(0, chars);

            if (chars.Length > 0 && chars[0] == '\n')
            {
                var line = new string(lineBuilder.ToArray()).Trim();

                if (ProcessLineAndStop(line))
                {
                    return _linesOfInterest;
                }

                lineBuilder.Clear();
            }

            try
            {
                stream.Seek(-_bufferLength, SeekOrigin.Current);
            }
            catch
            {
                break;
            }
        }

        _stopwatch.Stop();
        _logger.LogDebug("Couldn't find any event data tokens after reading {maxLines} lines in {numSeconds}s",
            MaxLinesToRead, _stopwatch.Elapsed.TotalSeconds);

        return _linesOfInterest;
    }

    private bool ProcessLineAndStop(string line)
    {
        if (ContainsEventToken(line))
        {
            _linesOfInterest.Add(line);

            if (_linesOfInterest.Count >= MaxLinesOfInterestToRead)
            {
                _stopwatch.Stop();
                _logger.LogDebug("Read {linesRead} lines in {numSeconds}s",
                    _linesOfInterest.Count, _stopwatch.Elapsed.TotalSeconds);

                return true;
            }
        }

        _linesRead++;

        if (_linesRead >= MaxLinesToRead)
        {
            _stopwatch.Stop();
            _logger.LogDebug("Read maximum number of lines ({linesRead}) in {numSeconds}s",
                _linesRead, _stopwatch.Elapsed.TotalSeconds);

            return true;
        }

        return false;
    }

    internal static bool ContainsEventToken(string line) => line.Contains(EventDataTokens.BaseToken);
}

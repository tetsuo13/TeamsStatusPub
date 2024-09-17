using System.Collections.Generic;
using Avalonia.Logging;
using Serilog;
using TeamsStatusPub.Core.Configuration;

namespace TeamsStatusPub;

/// <summary>
/// Log sink that passes everything over to Serilog. Configuring Serilog and
/// its sinks should be out of scope for this class.
/// </summary>
public class AvaloniaSerilogSink : ILogSink
{
    /// <summary>
    /// Assumes that <see cref="LoggingConfiguration.CreateDefaultLogger"/> has
    /// been called before an instance of this class has been created.
    /// </summary>
    private readonly ILogger _logger = LoggingConfiguration.Logger;

    /// <summary>
    /// <para>
    /// List of sources and their minimum log event level. Intended to skip
    /// logging if a source attempts to log at a level that is lesser than the
    /// override.
    /// </para>
    /// <para>
    /// This exists because calling
    /// <c>new LoggerConfiguration().MinimumLevel.Override("Avalonia.Layout.LayoutManager", LogEventLevel.Warning)</c>
    /// does not work as expected.
    /// </para>
    /// </summary>
    private readonly Dictionary<string, LogEventLevel> _overrides = new()
    {
        { "Avalonia.Layout.LayoutManager", LogEventLevel.Warning }
    };

    public bool IsEnabled(LogEventLevel level, string area) => _logger.IsEnabled(ToSerilogLogLevel(level));

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
    {
        if (InOverrideBelowMinLevel(source, level))
        {
            return;
        }
        _logger.Write(ToSerilogLogLevel(level), $"[{area} {source}] {messageTemplate}");
    }

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate,
        params object?[] propertyValues)
    {
        if (InOverrideBelowMinLevel(source, level))
        {
            return;
        }
        _logger.Write(ToSerilogLogLevel(level), $"[{area} {source}] {messageTemplate}", propertyValues);
    }

    private static Serilog.Events.LogEventLevel ToSerilogLogLevel(LogEventLevel level) =>
        (Serilog.Events.LogEventLevel)level;

    /// <summary>
    /// Checks whether the given source is listed in the
    /// <see cref="_overrides"/> collection and the requested log level is
    /// less than the minimum level listed there.
    /// </summary>
    /// <param name="source">The object name.</param>
    /// <param name="level">The requested log event level.</param>
    /// <returns>Whether source and level is less than what's in overrides.</returns>
    private bool InOverrideBelowMinLevel(object? source, LogEventLevel level)
    {
        if (source is null)
        {
            return false;
        }

        var sourceString = source.ToString();
        return !string.IsNullOrEmpty(sourceString) &&
            _overrides.TryGetValue(sourceString, out var minLevel) &&
            level < minLevel;
    }
}

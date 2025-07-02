using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.ReactiveUI;
using Serilog;
using TeamsStatusPub.Core.Configuration;

[assembly: Guid("73e16404-d5b2-4298-be82-051de8a5159e")]

namespace TeamsStatusPub;

internal static class Program
{
    [STAThread]
    public static async Task<int> Main(string[] args)
    {
        try
        {
            LoggingConfiguration.CreateDefaultLogger();
            Log.Information("Starting up...");

            return BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Application terminated unexpectedly");

            // Notifying the user of a fatal exception that likely caused the
            // application to unexpectedly close or not even start up would
            // improve the user experience.
            //
            // MessageBox is a classic goto, but it's not available yet.
            // See https://github.com/AvaloniaUI/Avalonia/issues/670

            return 1;
        }
        finally
        {
            Log.Information("Shutting down...");
            await Log.CloseAndFlushAsync();
        }
    }

    private static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseReactiveUI();
}

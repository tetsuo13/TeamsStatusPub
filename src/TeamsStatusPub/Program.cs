using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TeamsStatusPub.Configuration;
using TeamsStatusPub.Views;

namespace TeamsStatusPub;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        LoggingConfiguration.CreateDefaultLogger();

        var host = Host.CreateDefaultBuilder()
            .ConfigureAppLogging()
            .ConfigureAppServices()
            .Build();

        ApplicationConfiguration.Initialize();

        try
        {
            Log.Information("Starting application");
            Application.Run((Form)host.Services.GetRequiredService<IMainForm>());
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

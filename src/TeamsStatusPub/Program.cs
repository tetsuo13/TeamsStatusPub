using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TeamsStatusPub.Core.Configuration;
using TeamsStatusPub.Views;

[assembly: Guid("73e16404-d5b2-4298-be82-051de8a5159e")]

namespace TeamsStatusPub;

internal static class Program
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static IServiceProvider ServiceProvider { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        try
        {
            LoggingConfiguration.CreateDefaultLogger();
            Log.Debug("Initializing...");

            var host = Host.CreateDefaultBuilder()
                .ConfigureAppLogging()
                .ConfigureAppServices()
                .ConfigureServices(services =>
                {
                    services.AddTransient<MainForm>();
                    services.AddTransient<AboutForm>();
                })
                .Build();

            ServiceProvider = host.Services;

            ApplicationConfiguration.Initialize();

            Log.Information("Starting application");
            Application.Run(ServiceProvider.GetRequiredService<MainForm>());
        }
        catch (Exception e)
        {
            Log.Fatal(e, "Application terminated unexpectedly");
            MessageBox.Show($"An unexpected exception was encountered:\n{e.Message}",
                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}

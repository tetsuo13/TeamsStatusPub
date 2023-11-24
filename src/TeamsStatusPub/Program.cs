using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using TeamsStatusPub.Configuration;
using TeamsStatusPub.Views;

[assembly: Guid("73e16404-d5b2-4298-be82-051de8a5159e")]

namespace TeamsStatusPub;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        try
        {
            //LoggingConfiguration.CreateDefaultLogger();

            var host = Host.CreateDefaultBuilder()
                .ConfigureAppLogging()
                .ConfigureAppServices()
                .Build();

            ApplicationConfiguration.Initialize();

            Log.Information("Starting application");

            Application.Run((Form)host.Services.GetRequiredService<IMainForm>());
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

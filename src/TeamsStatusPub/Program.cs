using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TeamsStatusPub.Configuration;
using TeamsStatusPub.Views;

[assembly: InternalsVisibleTo("TeamsStatusPub.Tests")]

namespace TeamsStatusPub;

internal static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        var host = Host.CreateDefaultBuilder()
            .ConfigureAppServices()
            .Build();

        ApplicationConfiguration.Initialize();

        Application.Run((Form)host.Services.GetRequiredService<IMainForm>());
    }
}

using System;
using System.Linq;
using System.Reactive.Concurrency;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using TeamsStatusPub.Core.Configuration;
using TeamsStatusPub.Core.Services;
using TeamsStatusPub.Core.Services.AvailabilityHandlers;
using TeamsStatusPub.ViewModels;
using TeamsStatusPub.Views;

namespace TeamsStatusPub;

public class App : Application
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public static ServiceProvider ServiceProvider { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    private bool _isShowingAboutWindow;

    public override void Initialize()
    {
        Logger.Sink = new AvaloniaSerilogSink();
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.ConfigureAppServices();
        collection.AddTransient<AboutViewModel>();
        collection.AddTransient<AboutWindow>();

        ServiceProvider = collection.BuildServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            var listener = RxApp.MainThreadScheduler.Schedule(StartWebServer);

            desktop.Exit += (_, _) =>
            {
                listener.Dispose();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static async void StartWebServer()
    {
        var httpHost = ServiceProvider.GetRequiredService<IHttpProvider>();
        var availabilityHandler = ServiceProvider.GetRequiredService<IAvailabilityHandler>();
        await httpHost.Listen(availabilityHandler.IsAvailable);
    }

    /// <summary>
    /// Show About window as if it were a dialog. Since this is an icon tray
    /// application, there isn't a main window for the standard ShowDialog()
    /// method to attach to so mimic the behavior by only allowing one
    /// instance of the About window to be open at a time.
    /// </summary>
    private void AboutMenuClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime)
        {
            return;
        }

        var logger = ServiceProvider.GetRequiredService<ILogger<App>>();

        try
        {
            if (!_isShowingAboutWindow)
            {
                var view = ServiceProvider.GetRequiredService<AboutWindow>();
                view.Closing += (_, _) => _isShowingAboutWindow = false;
                view.Show();
                _isShowingAboutWindow = true;
            }
            else
            {
                logger.LogDebug("About window should already be open, bringing it to front");

                // Try to find the already-opened window.
                var view = ((IClassicDesktopStyleApplicationLifetime)ApplicationLifetime).Windows
                    .SingleOrDefault(x => x is AboutWindow);

                if (view is null)
                {
                    logger.LogWarning("Unable to find About window");
                }
                else
                {
                    view.Activate();
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while showing About window");
        }
    }

    private void ExitMenuClick(object? sender, EventArgs e)
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.Shutdown();
        }
    }
}

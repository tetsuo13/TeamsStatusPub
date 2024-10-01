using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using TeamsStatusPub.Views;

namespace TeamsStatusPub.ViewModels;

public class AppViewModel : ViewModelBase
{
    public ReactiveCommand<Unit, Unit> AboutCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }

    public AppViewModel()
    {
        AboutCommand = ReactiveCommand.Create(() =>
        {
            // Treat the about window like a dialog: only one instance should
            // ever be shown.
            var view = App.ServiceProvider.GetRequiredService<AboutWindow>();

            if (!view.IsVisible)
            {
                view.Show();
            }
            else
            {
                view.Activate();
            }
        });

        ExitCommand = ReactiveCommand.Create(() =>
        {
            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            {
                lifetime.Shutdown();
            }
        });
    }
}

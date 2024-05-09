using System.Reactive;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
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
            var view = new AboutWindow();
            view.Show();
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

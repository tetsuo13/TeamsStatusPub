using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using TeamsStatusPub.ViewModels;

namespace TeamsStatusPub.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<AboutViewModel>();
    }
}

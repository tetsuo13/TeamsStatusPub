using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeamsStatusPub.Core.Presenters;
using TeamsStatusPub.Properties;

namespace TeamsStatusPub.Views;

public partial class MainForm : Form
{
    private readonly NotifyIcon _notifyIcon;

    public MainForm(ILogger<MainForm> logger, IMainFormPresenter presenter)
    {
        ArgumentNullException.ThrowIfNull(presenter);
        ArgumentNullException.ThrowIfNull(logger);

        InitializeComponent();

        try
        {
            _notifyIcon = new NotifyIcon
            {
                Text = presenter.ApplicationName,
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = Resources.MainIcon
            };
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error setting up notification icon");
            throw;
        }

        _notifyIcon.ContextMenuStrip.Items.Add("About");
        _notifyIcon.ContextMenuStrip.Items[0].Click += ContextMenu_About;
        _notifyIcon.ContextMenuStrip.Items.Add("Quit");
        _notifyIcon.ContextMenuStrip.Items[1].Click += ContextMenu_Quit;
        _notifyIcon.Visible = true;

        WindowState = FormWindowState.Minimized;
        ShowInTaskbar = false;

        Load += (object? sender, EventArgs e) => Task.Run(() => presenter.OnViewLoad());
        FormClosing += (object? sender, FormClosingEventArgs e) => RemoveNotifyIcon();
    }

    protected override void OnShown(EventArgs e)
    {
        Visible = false;
    }

    private void ContextMenu_Quit(object? sender, EventArgs e)
    {
        RemoveNotifyIcon();
        Application.Exit();
    }

    private void ContextMenu_About(object? sender, EventArgs e)
    {
        using var form = Program.ServiceProvider.GetRequiredService<AboutForm>();
        form.ShowDialog();
    }

    private void RemoveNotifyIcon()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}

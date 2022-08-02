﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TeamsStatusPub.Presenters;

namespace TeamsStatusPub.Views;

public partial class MainForm : Form, IMainForm
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly NotifyIcon _notifyIcon;

    public MainForm(ILogger<MainForm> logger, IServiceScopeFactory serviceScopeFactory, IMainFormPresenter presenter)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));

        if (presenter is null)
        {
            throw new ArgumentNullException(nameof(presenter));
        }

        if (logger is null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        InitializeComponent();

        try
        {
            var iconPath = Path.Combine(presenter.ApplicationPath, "logo.ico");

            _notifyIcon = new NotifyIcon
            {
                Text = presenter.ApplicationName,
                ContextMenuStrip = new ContextMenuStrip(),
                Icon = new Icon(iconPath)
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
        Visible = false;
        ShowInTaskbar = false;

        Load += (object? sender, EventArgs e) => Task.Run(() => presenter.OnViewLoad());
        FormClosing += (object? sender, FormClosingEventArgs e) => RemoveNotifyIcon();
    }

    private void ContextMenu_Quit(object? sender, EventArgs e)
    {
        RemoveNotifyIcon();
        Application.Exit();
    }

    private void ContextMenu_About(object? sender, EventArgs e)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        using var form = (Form)scope.ServiceProvider.GetRequiredService<IAboutForm>();
        form.ShowDialog();
    }

    private void RemoveNotifyIcon()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}

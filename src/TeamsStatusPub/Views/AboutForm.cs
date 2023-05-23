using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TeamsStatusPub.Presenters;

namespace TeamsStatusPub.Views;

public partial class AboutForm : Form, IAboutForm
{
    private readonly IAboutFormPresenter _presenter;
    private readonly ILogger<AboutForm> _logger;

    public AboutForm(IAboutFormPresenter presenter, ILogger<AboutForm> logger)
    {
        _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        InitializeComponent();

        Text = $"About {presenter.ApplicationName}";
        productLabel.Text = presenter.ApplicationName;
        copyrightLabel.Text = presenter.Copyright;
        versionLabel.Text = presenter.Version;
        teamsStatusTooltip.SetToolTip(teamsStatusLabel,
            """
            The status shown is not live and does not change here.
            It's simply the status at the time of opening this dialog.
            """);

        StyleLinkLabel(websiteLinkLabel, presenter.WebsiteUrl);
        StyleLinkLabel(listenLinkLabel, presenter.ListenUrl);
    }

    protected override void OnLoad(EventArgs e)
    {
        lastTeamsStatusLabel.Text = _presenter.LastTeamsStatus;
    }

    private void StyleLinkLabel(LinkLabel label, string url)
    {
        label.Text = url;
        label.Links.Add(new LinkLabel.Link(0, url.Length, url));

        label.MouseEnter += (object? sender, EventArgs e) => label.LinkColor = Color.Red;
        label.MouseLeave += (object? sender, EventArgs e) => label.LinkColor = Color.Blue;

        label.LinkClicked += (object? sender, LinkLabelLinkClickedEventArgs e) =>
        {
            try
            {
                var url = e.Link!.LinkData.ToString();
                var startInfo = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error opening link to website on {labelName}", label.Name);
            }
        };
    }
}

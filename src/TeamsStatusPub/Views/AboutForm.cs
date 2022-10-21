using System.Diagnostics;
using Microsoft.Extensions.Logging;
using TeamsStatusPub.Presenters;

namespace TeamsStatusPub.Views;

public partial class AboutForm : Form, IAboutForm
{
    public AboutForm(IAboutFormPresenter presenter, ILogger<AboutForm> logger)
    {
        ArgumentNullException.ThrowIfNull(presenter);
        ArgumentNullException.ThrowIfNull(logger);

        InitializeComponent();

        Text = $"About {presenter.ApplicationName}";
        productLabel.Text = presenter.ApplicationName;
        copyrightLabel.Text = presenter.Copyright;
        versionLabel.Text = presenter.Version;

        websiteLinkLabel.Text = presenter.WebsiteUrl;
        websiteLinkLabel.Links.Add(new LinkLabel.Link(0, presenter.WebsiteUrl.Length, presenter.WebsiteUrl));

        websiteLinkLabel.LinkClicked += (object sender, LinkLabelLinkClickedEventArgs e) =>
        {
            try
            {
                var url = e.Link.LinkData.ToString();
                var startInfo = new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error opening link to website");
            }
        };
    }
}

using TeamsStatusPub.Services;

namespace TeamsStatusPub.Presenters;

public class AboutFormPresenter : IAboutFormPresenter
{
    public string ApplicationName => _appInfo.ApplicationName;
    public string Copyright => _appInfo.Copyright;
    public string WebsiteUrl => _appInfo.WebsiteUrl;
    public string Version => _appInfo.Version;

    private readonly IAppInfo _appInfo;

    public AboutFormPresenter(IAppInfo appInfo)
    {
        _appInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
    }
}

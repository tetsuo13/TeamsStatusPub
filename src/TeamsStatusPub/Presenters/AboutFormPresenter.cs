using Microsoft.Extensions.Options;
using TeamsStatusPub.Models;
using TeamsStatusPub.Services;

namespace TeamsStatusPub.Presenters;

public class AboutFormPresenter : IAboutFormPresenter
{
    public string ApplicationName => _appInfo.ApplicationName;
    public string Copyright => _appInfo.Copyright;
    public string WebsiteUrl => _appInfo.WebsiteUrl;
    public string Version => _appInfo.Version;
    public string ListenUrl => $"http://{_runtimeSettings.Value.ListenAddress}:{_runtimeSettings.Value.ListenPort}/";

    private readonly IAppInfo _appInfo;
    private readonly IOptions<RuntimeSettings> _runtimeSettings;

    public AboutFormPresenter(IAppInfo appInfo, IOptions<RuntimeSettings> runtimeSettings)
    {
        _appInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
        _runtimeSettings = runtimeSettings ?? throw new ArgumentNullException(nameof(runtimeSettings));
    }
}

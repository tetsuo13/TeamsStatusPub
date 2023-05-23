using Microsoft.Extensions.Options;
using TeamsStatusPub.Models;
using TeamsStatusPub.Services;
using TeamsStatusPub.Services.AvailabilityHandlers;

namespace TeamsStatusPub.Presenters;

public class AboutFormPresenter : IAboutFormPresenter
{
    public string ApplicationName => _appInfo.ApplicationName;
    public string Copyright => _appInfo.Copyright;
    public string WebsiteUrl => _appInfo.WebsiteUrl;
    public string Version => _appInfo.Version;
    public string ListenUrl => $"http://{_runtimeSettings.Value.ListenAddress}:{_runtimeSettings.Value.ListenPort}/";
    public string LastTeamsStatus => _availabilityHandler.IsAvailable() ? "not busy" : "busy";

    private readonly IAppInfo _appInfo;
    private readonly IOptions<RuntimeSettings> _runtimeSettings;
    private readonly IAvailabilityHandler _availabilityHandler;

    public AboutFormPresenter(IAppInfo appInfo, IOptions<RuntimeSettings> runtimeSettings,
        IAvailabilityHandler availabilityHandler)
    {
        _appInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
        _runtimeSettings = runtimeSettings ?? throw new ArgumentNullException(nameof(runtimeSettings));
        _availabilityHandler = availabilityHandler ?? throw new ArgumentNullException(nameof(availabilityHandler));
    }
}

using System.ComponentModel;
using System.Reflection;
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

    public string LastAvailabilitySystemStatus
    {
        get
        {
            var status = _availabilityHandler.IsAvailable() ? "not busy" : "busy";
            var handlerName = _runtimeSettings.Value.AvailabilityHandler
                .GetType()
                .GetMember(_runtimeSettings.Value.AvailabilityHandler.ToString())[0]
                .GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;

            return handlerName is null
                ? throw new NotImplementedException("Missing expected Description attribute")
                : $"{handlerName.Description}: {status}";
        }
    }

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

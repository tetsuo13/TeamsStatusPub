using System;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.Options;
using TeamsStatusPub.Core.Models;
using TeamsStatusPub.Core.Services;
using TeamsStatusPub.Core.Services.AvailabilityHandlers;

namespace TeamsStatusPub.ViewModels;

public class AboutViewModel : ViewModelBase
{
    public string ApplicationName => _appInfo.ApplicationName;
    public string Copyright => _appInfo.Copyright;
    public string WebsiteUrl => _appInfo.WebsiteUrl;
    public string Version => _appInfo.Version;
    public string ListenUrl => $"http://{_runtimeSettings.ListenAddress}:{_runtimeSettings.ListenPort}/";

    public string LastAvailabilitySystemStatus
    {
        get
        {
            var status = _availabilityHandler.IsAvailable() ? "not busy" : "busy";
            var handlerName = _runtimeSettings.AvailabilityHandler
                .GetType()
                .GetMember(_runtimeSettings.AvailabilityHandler.ToString())[0]
                .GetCustomAttribute(typeof(DescriptionAttribute), false) as DescriptionAttribute;

            return handlerName is null
                ? throw new NotImplementedException("Missing expected Description attribute")
                : $"{handlerName.Description}: {status}";
        }
    }

    private readonly IAppInfo _appInfo;
    private readonly RuntimeSettings _runtimeSettings;
    private readonly IAvailabilityHandler _availabilityHandler;

    public AboutViewModel(IAppInfo appInfo, IOptions<RuntimeSettings> runtimeSettings,
        IAvailabilityHandler availabilityHandler)
    {
        ArgumentNullException.ThrowIfNull(runtimeSettings);
        _appInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
        _runtimeSettings = runtimeSettings.Value ?? throw new ArgumentNullException(nameof(runtimeSettings));
        _availabilityHandler = availabilityHandler ?? throw new ArgumentNullException(nameof(availabilityHandler));
    }
}

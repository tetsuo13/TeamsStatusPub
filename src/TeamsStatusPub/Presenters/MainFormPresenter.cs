using Microsoft.Extensions.Logging;
using TeamsStatusPub.Services;
using TeamsStatusPub.Services.AvailabilityHandlers;

namespace TeamsStatusPub.Presenters;

public class MainFormPresenter : IMainFormPresenter
{
    public string ApplicationName => _appInfo.ApplicationName;

    private readonly ILogger<MainFormPresenter> _logger;
    private readonly IHttpProvider _httpHost;
    private readonly IAvailabilityHandler _availabilityHandler;
    private readonly IAppInfo _appInfo;

    public MainFormPresenter(ILogger<MainFormPresenter> logger, IHttpProvider httpHost,
        IAvailabilityHandler availabilityHandler, IAppInfo appInfo)
    {
        _httpHost = httpHost ?? throw new ArgumentNullException(nameof(httpHost));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _availabilityHandler = availabilityHandler ?? throw new ArgumentNullException(nameof(availabilityHandler));
        _appInfo = appInfo ?? throw new ArgumentNullException(nameof(appInfo));
    }

    public async Task OnViewLoad()
    {
        _logger.LogInformation("Listening for incoming connections...");
        await _httpHost.Listen(_availabilityHandler.IsAvailable);
    }
}

namespace TeamsStatusPub.Core.Presenters;

public interface IAboutFormPresenter
{
    string ApplicationName { get; }
    string Copyright { get; }
    string Version { get; }
    string WebsiteUrl { get; }
    string ListenUrl { get; }
    string LastAvailabilitySystemStatus { get; }
}

namespace TeamsStatusPub.Presenters;

public interface IMainFormPresenter
{
    string ApplicationName { get; }
    string ApplicationPath { get; }
    Task OnViewLoad();
}

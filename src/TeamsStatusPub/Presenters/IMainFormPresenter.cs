namespace TeamsStatusPub.Presenters;

public interface IMainFormPresenter
{
    string ApplicationName { get; }
    Task OnViewLoad();
}

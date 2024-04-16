namespace TeamsStatusPub.Core.Presenters;

public interface IMainFormPresenter
{
    string ApplicationName { get; }
    Task OnViewLoad();
}

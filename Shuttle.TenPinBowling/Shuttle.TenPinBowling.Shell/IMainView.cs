namespace Shuttle.TenPinBowling.Shell;

public interface IMainView
{
    void Assign(IMainPresenter presenter, IModel model);
    void GameFinished();
    void ShowMessage(string message);
}
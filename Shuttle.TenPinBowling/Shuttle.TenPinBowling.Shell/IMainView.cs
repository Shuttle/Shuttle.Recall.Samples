namespace Shuttle.TenPinBowling.Shell
{
	public interface IMainView
	{
		void Assign(IMainPresenter presenter, IModel model);
		void ShowMessage(string message);
		void GameFinished();
	}
}
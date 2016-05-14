namespace Shuttle.TenPinBowling.Shell
{
	public interface IMainView
	{
		void Assign(MainPresenter presenter, IModel model);
		void ShowMessage(string message);
	}
}
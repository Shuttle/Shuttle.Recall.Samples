using Shuttle.Recall;

namespace Shuttle.TenPinBowling.Shell
{
	public class MainPresenter : IMainPresenter
	{
		private readonly IMainView _view;
		private readonly IEventStore _eventStore;
		private readonly IModel _model = new Model();

		public MainPresenter(IMainView view, IEventStore eventStore)
		{
			_view = view;
			_eventStore = eventStore;

			view.Assign(this, _model);
		}

		public void Roll(int pins)
		{
			if (!_model.HasGameStarted)
			{
				_view.ShowMessage("No game has been started.");

				return;
			}
		}

		public void StartGame(string bowler)
		{
			if (string.IsNullOrEmpty(bowler))
			{
				_view.ShowMessage("Enter a bowler name.");

				return;
			}

			_model.OnGameStarted(bowler);
		}
	}
}
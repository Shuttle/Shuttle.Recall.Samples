using Shuttle.Core.Data;
using Shuttle.Recall;

namespace Shuttle.TenPinBowling.Shell
{
    public class MainPresenter : IMainPresenter
    {
        private Game _game;
        private readonly IMainView _view;
        private readonly ITenPinBowlingDatabaseContextFactory _databaseContextFactory;
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IEventStore _eventStore;
        private readonly IBowlingQueryFactory _bowlingQueryFactory;
        private readonly IModel _model = new Model();

        public MainPresenter(IMainView view, ITenPinBowlingDatabaseContextFactory databaseContextFactory, IDatabaseGateway databaseGateway, IEventStore eventStore, IBowlingQueryFactory bowlingQueryFactory)
        {
            _view = view;
            _databaseContextFactory = databaseContextFactory;
            _databaseGateway = databaseGateway;
            _eventStore = eventStore;
            _bowlingQueryFactory = bowlingQueryFactory;

            view.Assign(this, _model);

            FetchGames();
        }

        private void FetchGames()
        {
            using (_databaseContextFactory.Create())
            {
                foreach (var row in _databaseGateway.GetRowsUsing(_bowlingQueryFactory.All()))
                {
                    _model.OnGameAdded(
                        GameColumns.Id.MapFrom(row),
                        GameColumns.Bowler.MapFrom(row),
                        GameColumns.DateStarted.MapFrom(row)
                        );
                }
            }
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

            _game = new Game();


            using (_databaseContextFactory.Create())
            {
                var eventStream = new EventStream(_game.Id);

                eventStream.AddEvent(_game.Start(bowler));

                _eventStore.SaveEventStream(eventStream);
            }

            _model.OnGameStarted(bowler);
        }
    }
}
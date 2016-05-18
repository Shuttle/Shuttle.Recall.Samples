using System;
using Shuttle.Core.Data;
using Shuttle.Recall;

namespace Shuttle.TenPinBowling.Shell
{
    public class MainPresenter : IMainPresenter
    {
        private readonly IBowlingQueryFactory _bowlingQueryFactory;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IDatabaseGateway _databaseGateway;
        private readonly IEventStore _eventStore;
        private readonly IModel _model = new Model();
        private readonly IMainView _view;
        private Game _game;

        public MainPresenter(IMainView view, IDatabaseContextFactory databaseContextFactory,
            IDatabaseGateway databaseGateway, IEventStore eventStore, IBowlingQueryFactory bowlingQueryFactory)
        {
            _view = view;
            _databaseContextFactory = databaseContextFactory;
            _databaseGateway = databaseGateway;
            _eventStore = eventStore;
            _bowlingQueryFactory = bowlingQueryFactory;

            view.Assign(this, _model);

            FetchGames();
        }

        public void Roll(int pins)
        {
            if (!_model.HasGameStarted)
            {
                _view.ShowMessage("No game has been started.");

                return;
            }

            try
            {
                var pinfall = _game.Roll(pins);

                using (_databaseContextFactory.Create(Connections.EventStore))
                {
                    _eventStore.SaveEventStream(
                        _eventStore.Get(_game.Id)
                            .AddEvent(pinfall)
                        );
                }

                _model.AddFrameScore(pinfall.Frame, pinfall.Roll, pinfall.Pins);
            }
            catch (Exception ex)
            {
                _view.ShowMessage(ex.Message);
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

            using (_databaseContextFactory.Create(Connections.EventStore))
            {
                var stream = new EventStream(_game.Id);

                stream.AddEvent(_game.Start(bowler));

                _eventStore.SaveEventStream(stream);
            }

            _model.OnGameStarted(bowler);
            _model.AddGame(_game.Id, _model.Bowler, DateTime.Now);
        }

        public void SelectGame(Guid id)
        {
            _game = new Game(id);

            using (_databaseContextFactory.Create(Connections.EventStore))
            {
                _eventStore.Get(id).Apply(_game);
            }

            using (_databaseContextFactory.Create(Connections.Projection))
            {
                _model.OnGameStarted(
                    GameColumns.Bowler.MapFrom(
                        _databaseGateway.GetSingleRowUsing(_bowlingQueryFactory.GetGame(id))
                        ));
            }
        }

        private void FetchGames()
        {
            using (_databaseContextFactory.Create(Connections.Projection))
            {
                foreach (var row in _databaseGateway.GetRowsUsing(_bowlingQueryFactory.All()))
                {
                    _model.AddGame(
                        GameColumns.Id.MapFrom(row),
                        GameColumns.Bowler.MapFrom(row),
                        GameColumns.DateStarted.MapFrom(row)
                        );
                }
            }
        }
    }
}
using System;
using Shuttle.Core.Data;
using Shuttle.Recall;

namespace Shuttle.TenPinBowling.Shell
{
    public class MainPresenter : IMainPresenter
    {
        private readonly IBowlingQuery _bowlingQuery;
        private readonly IDatabaseContextFactory _databaseContextFactory;
        private readonly IEventStore _eventStore;
        private readonly IModel _model = new Model();
        private readonly IMainView _view;
        private Game _game;

        public MainPresenter(IMainView view, IDatabaseContextFactory databaseContextFactory, IEventStore eventStore,
            IBowlingQuery bowlingQuery)
        {
            _view = view;
            _databaseContextFactory = databaseContextFactory;
            _eventStore = eventStore;
            _bowlingQuery = bowlingQuery;

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
                    _eventStore.Save(
                        _eventStore.Get(_game.Id)
                            .AddEvent(pinfall)
                        );
                }

                _model.AddFrameScore(pinfall.Frame, pinfall.FrameRoll, pinfall.Pins, pinfall.Strike, pinfall.Spare,
                     pinfall.StandingPins);

                foreach (var frameBonus in pinfall.FrameBonuses)
                {
                    _model.AddFrameBonusScore(frameBonus, pins);
                }
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

                _eventStore.Save(stream);
            }

            _model.StartGame(bowler);
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
                _model.StartGame(GameColumns.Bowler.MapFrom(_bowlingQuery.GetGame(id)));

                foreach (var row in _bowlingQuery.GameFrames(id))
                {
                    _model.AddFrameScore(
                        FrameColumns.Frame.MapFrom(row),
                        FrameColumns.FrameRoll.MapFrom(row),
                        FrameColumns.Pins.MapFrom(row),
                        FrameColumns.Strike.MapFrom(row) == 1,
                        FrameColumns.Spare.MapFrom(row) == 1,
                        FrameColumns.StandingPins.MapFrom(row));
                }

                foreach (var row in _bowlingQuery.GameFrameBonuses(id))
                {
                    _model.AddFrameBonusScore(
                        FrameBonusColumns.BonusFrame.MapFrom(row),
                        FrameBonusColumns.BonusPins.MapFrom(row));
                }
            }
        }

        private void FetchGames()
        {
            using (_databaseContextFactory.Create(Connections.Projection))
            {
                foreach (var row in _bowlingQuery.AllGames())
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
using System;
using System.Threading.Tasks;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.Recall;

namespace Shuttle.TenPinBowling.Shell;

public class MainPresenter : IMainPresenter
{
    private readonly IBowlingQuery _bowlingQuery;
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly IEventStore _eventStore;
    private readonly IModel _model = new Model();
    private readonly IMainView _view;
    private Game _game = new();

    public MainPresenter(IMainView view, IDatabaseContextFactory databaseContextFactory, IEventStore eventStore, IBowlingQuery bowlingQuery)
    {
        _view = view;
        _databaseContextFactory = databaseContextFactory;
        _eventStore = eventStore;
        _bowlingQuery = bowlingQuery;

        view.Assign(this, _model);

        _ = FetchGamesAsync();
    }

    public async Task RollAsync(int pins)
    {
        if (!_model.HasGameStarted)
        {
            _view.ShowMessage("No game has been started.");

            return;
        }

        try
        {
            var pinfall = _game.Roll(pins);

            await using (_databaseContextFactory.Create(Connections.EventStore))
            {
                await _eventStore.SaveAsync((await _eventStore.GetAsync(_game.Id)).Add(pinfall));
            }

            _model.AddFrameScore(pinfall.Frame, pinfall.FrameRoll, pinfall.Pins, pinfall.Strike, pinfall.Spare, pinfall.StandingPins);

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

    public async Task StartGameAsync(string bowler)
    {
        if (string.IsNullOrEmpty(bowler))
        {
            _view.ShowMessage("Enter a bowler name.");

            return;
        }

        _game = new();

        await using (_databaseContextFactory.Create(Connections.EventStore))
        {
            var stream = await _eventStore.GetAsync(_game.Id);

            stream.Add(_game.Start(bowler));

            await _eventStore.SaveAsync(stream);
        }

        _model.StartGame(bowler);
        _model.AddGame(_game.Id, _model.Bowler, DateTime.Now);
    }

    public async Task SelectGameAsync(Guid id)
    {
        _game = new(id);

        await using (_databaseContextFactory.Create(Connections.EventStore))
        {
            (await _eventStore.GetAsync(id)).Apply(_game);
        }

        await using (_databaseContextFactory.Create(Connections.Projection))
        {
            var gameRow = await _bowlingQuery.FindGameAsync(id);

            if (gameRow == null)
            {
                _view.ShowMessage("Could not find game.");

                return;
            }

            _model.StartGame(Guard.AgainstNullOrEmptyString(Columns.Bowler.Value(gameRow)));

            foreach (var row in await _bowlingQuery.GameFramesAsync(id))
            {
                _model.AddFrameScore(
                    Columns.Frame.Value(row),
                    Columns.FrameRoll.Value(row),
                    Columns.Pins.Value(row),
                    Columns.Strike.Value(row) == 1,
                    Columns.Spare.Value(row) == 1,
                    Columns.StandingPins.Value(row));
            }

            foreach (var row in await _bowlingQuery.GameFrameBonusesAsync(id))
            {
                _model.AddFrameBonusScore(
                    Columns.BonusFrame.Value(row),
                    Columns.BonusPins.Value(row));
            }
        }

        if (_game.Finished)
        {
            _view.GameFinished();
        }
    }

    private async Task FetchGamesAsync()
    {
        await using (_databaseContextFactory.Create(Connections.Projection))
        {
            foreach (var row in await _bowlingQuery.AllGamesAsync())
            {
                _model.AddGame(
                    Columns.Id.Value(row),
                    Guard.AgainstNullOrEmptyString(Columns.Bowler.Value(row)),
                    Columns.DateStarted.Value(row)
                );
            }
        }
    }
}
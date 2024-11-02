using System;

namespace Shuttle.TenPinBowling.Shell;

public class GameAddedEventArgs
{
    public GameAddedEventArgs(Guid id, string bowler, DateTime dateStarted)
    {
        Id = id;
        Bowler = bowler;
        DateStarted = dateStarted;
    }

    public string Bowler { get; private set; }
    public DateTime DateStarted { get; private set; }
    public Guid Id { get; private set; }
}
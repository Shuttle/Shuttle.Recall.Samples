using System;

namespace Shuttle.TenPinBowling.Events.v1;

public class GameStarted
{
    public string Bowler { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
}
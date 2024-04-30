using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling.Shell;

public class GameColumns
{
    public static Column<Guid> Id = new("Id", DbType.Guid);
    public static Column<string> Bowler = new("Bowler", DbType.AnsiString, 65);
    public static Column<DateTime> DateStarted = new("DateStarted", DbType.DateTime);
}
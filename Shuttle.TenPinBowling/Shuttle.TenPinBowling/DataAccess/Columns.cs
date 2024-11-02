using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling;

public class Columns
{
    public static Column<Guid> GameId = new("GameId", DbType.Guid);
    public static Column<int> Frame = new("Frame", DbType.Int32);
    public static Column<int> BonusFrame = new("BonusFrame", DbType.Int32);
    public static Column<int> BonusPins = new("BonusPins", DbType.Int32);
    public static Column<int> FrameRoll = new("FrameRoll", DbType.Int32);
    public static Column<int> Pins = new("Pins", DbType.Int32);
    public static Column<int> Roll = new("Roll", DbType.Int32);
    public static Column<int> Score = new("Score", DbType.Int32);
    public static Column<int> BonusRolls = new("BonusRolls", DbType.Int32);
    public static Column<byte> FrameFinished = new("FrameFinished", DbType.Byte);
    public static Column<byte> Strike = new("Strike", DbType.Byte);
    public static Column<byte> Spare = new("Spare", DbType.Byte);
    public static Column<byte> Open = new("Open", DbType.Byte);
    public static Column<int> StandingPins = new("StandingPins", DbType.Int32);
    public static Column<byte> GameFinished = new("GameFinished", DbType.Byte);
    public static Column<Guid> Id = new("Id", DbType.Guid);
    public static Column<string> Bowler = new("Bowler", DbType.AnsiString, 65);
    public static Column<DateTime> DateStarted = new("DateStarted", DbType.DateTime);
}
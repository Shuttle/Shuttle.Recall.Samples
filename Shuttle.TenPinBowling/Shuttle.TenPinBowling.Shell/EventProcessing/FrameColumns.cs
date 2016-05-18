using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling.Shell
{
    public class FrameColumns
    {
        public static MappedColumn<Guid>  GameId = new MappedColumn<Guid>("GameId", DbType.Guid);
        public static MappedColumn<int> Frame = new MappedColumn<int>("Frame", DbType.Int32);
        public static MappedColumn<int> FrameRoll = new MappedColumn<int>("FrameRoll", DbType.Int32);
        public static MappedColumn<int> Pins = new MappedColumn<int>("Pins", DbType.Int32);
        public static MappedColumn<int> Roll = new MappedColumn<int>("Roll", DbType.Int32);
        public static MappedColumn<int> Score = new MappedColumn<int>("Score", DbType.Int32);
        public static MappedColumn<int> BonusRolls = new MappedColumn<int>("BonusRolls", DbType.Int32);
        public static MappedColumn<byte> FrameFinished = new MappedColumn<byte>("FrameFinished", DbType.Byte);
        public static MappedColumn<byte> Strike = new MappedColumn<byte>("Strike", DbType.Byte);
        public static MappedColumn<byte> Spare = new MappedColumn<byte>("Spare", DbType.Byte);
        public static MappedColumn<byte> Open = new MappedColumn<byte>("Open", DbType.Byte);
        public static MappedColumn<int> StandingPins = new MappedColumn<int>("StandingPins", DbType.Int32);
        public static MappedColumn<byte> GameFinished = new MappedColumn<byte>("GameFinished", DbType.Byte);
    }
}
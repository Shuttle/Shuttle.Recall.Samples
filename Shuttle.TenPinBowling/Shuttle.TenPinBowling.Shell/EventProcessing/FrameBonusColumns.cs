using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling.Shell
{
    public class FrameBonusColumns
    {
        public static Column<Guid>  GameId = new Column<Guid>("GameId", DbType.Guid);
        public static Column<int> Frame = new Column<int>("Frame", DbType.Int32);
        public static Column<int> BonusFrame = new Column<int>("BonusFrame", DbType.Int32);
        public static Column<int> BonusPins = new Column<int>("BonusPins", DbType.Int32);
    }
}
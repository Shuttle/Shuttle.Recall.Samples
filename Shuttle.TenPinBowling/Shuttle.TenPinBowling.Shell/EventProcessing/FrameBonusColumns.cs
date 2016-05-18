using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling.Shell
{
    public class FrameBonusColumns
    {
        public static MappedColumn<Guid>  GameId = new MappedColumn<Guid>("GameId", DbType.Guid);
        public static MappedColumn<int> Frame = new MappedColumn<int>("Frame", DbType.Int32);
        public static MappedColumn<int> BonusFrame = new MappedColumn<int>("BonusFrame", DbType.Int32);
        public static MappedColumn<int> BonusPins = new MappedColumn<int>("BonusPins", DbType.Int32);
    }
}
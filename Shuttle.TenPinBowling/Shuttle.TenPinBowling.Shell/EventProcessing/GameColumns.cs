using System;
using System.Data;
using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling.Shell
{
    public class GameColumns
    {
        public static MappedColumn<Guid>  Id = new MappedColumn<Guid>("Id", DbType.Guid);
        public static MappedColumn<string> Bowler = new MappedColumn<string>("Bowler", DbType.AnsiString, 65);
        public static MappedColumn<DateTime>  DateStarted = new MappedColumn<DateTime>("DateStarted", DbType.DateTime);
    }
}
using System;
using System.Threading.Tasks;

namespace Shuttle.TenPinBowling.Shell;

public interface IMainPresenter
{
    Task RollAsync(int pins);
    Task SelectGameAsync(Guid id);
    Task StartGameAsync(string bowler);
}
using System;

namespace Shuttle.TenPinBowling.Shell
{
	public interface IMainPresenter
	{
	    void Roll(int pins);
	    void StartGame(string bowler);
	    void SelectGame(Guid id);
	}
}
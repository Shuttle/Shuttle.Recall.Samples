using System;
using NUnit.Framework;

namespace Shuttle.TenPinBowling.Tests
{
	[TestFixture]
	public class GameFixture
	{
		[Test]
		public void Should_not_be_able_to_start_a_game_that_has_already_started()
		{
			var game = new Game();

			game.Start("Bowler");

			Assert.Throws<ApplicationException>(() => game.Start("Bowler"));
		}

		[Test]
		public void Should_not_be_able_to_roll_if_a_a_game_has_not_yet_started()
		{
			var game = new Game();

			Assert.Throws<ApplicationException>(() => game.Roll(5));
		}

		[Test]
		public void Should_be_able_to_complete_a_game_of_single_pin_throws()
		{
			var game = new Game();

			game.Start("Bowler");

			Assert.AreEqual(1, game.Roll(1).Frame);
			Assert.AreEqual(1, game.Roll(1).Frame);
			Assert.AreEqual(2, game.Roll(1).Frame);
			Assert.AreEqual(2, game.Roll(1).Frame);
			Assert.AreEqual(3, game.Roll(1).Frame);
			Assert.AreEqual(3, game.Roll(1).Frame);
			Assert.AreEqual(4, game.Roll(1).Frame);
			Assert.AreEqual(4, game.Roll(1).Frame);
			Assert.AreEqual(5, game.Roll(1).Frame);
			Assert.AreEqual(5, game.Roll(1).Frame);
			Assert.AreEqual(6, game.Roll(1).Frame);
			Assert.AreEqual(6, game.Roll(1).Frame);
			Assert.AreEqual(7, game.Roll(1).Frame);
			Assert.AreEqual(7, game.Roll(1).Frame);
			Assert.AreEqual(8, game.Roll(1).Frame);
			Assert.AreEqual(8, game.Roll(1).Frame);
			Assert.AreEqual(9, game.Roll(1).Frame);
			Assert.AreEqual(9, game.Roll(1).Frame);
			Assert.AreEqual(10, game.Roll(1).Frame);
			Assert.AreEqual(10, game.Roll(1).Frame);

			Assert.Throws<ApplicationException>(() => game.Roll(1));
		}

		[Test]
		public void Should_be_able_to_throw_a_strike()
		{
			var game = new Game();

			game.Start("Bowler");

			Assert.IsTrue(game.Roll(10).Strike);
		}

		[Test]
		public void Should_be_able_to_throw_a_spare()
		{
			var game = new Game();

			game.Start("Bowler");

			game.Roll(8);
			Assert.IsTrue(game.Roll(2).Spare);
		}

		[Test]
		public void Should_be_able_to_throw_a_open()
		{
			var game = new Game();

			game.Start("Bowler");

			game.Roll(2);
			Assert.IsTrue(game.Roll(2).Open);
		}

		[Test]
		public void Should_be_able_to_throw_a_perfect_game()
		{
			var game = new Game();

			game.Start("Bowler");

			game.Roll(10);
			game.Roll(10);
			game.Roll(10);
			game.Roll(10);
			game.Roll(10);
			game.Roll(10);
			game.Roll(10);
			game.Roll(10);
			game.Roll(10);
			game.Roll(10);
			game.Roll(10);
			Assert.AreEqual(300, game.Roll(10).Score);
		}
	}
}
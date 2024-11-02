using System;
using NUnit.Framework;

namespace Shuttle.TenPinBowling.Tests;

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

        Assert.That(game.Roll(1).Frame, Is.EqualTo(1));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(1));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(2));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(2));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(3));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(3));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(4));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(4));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(5));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(5));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(6));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(6));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(7));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(7));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(8));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(8));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(9));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(9));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(10));
        Assert.That(game.Roll(1).Frame, Is.EqualTo(10));

        Assert.Throws<ApplicationException>(() => game.Roll(1));
    }

    [Test]
    public void Should_be_able_to_throw_a_strike()
    {
        var game = new Game();

        game.Start("Bowler");

        Assert.That(game.Roll(10).Strike, Is.True);
    }

    [Test]
    public void Should_be_able_to_throw_a_spare()
    {
        var game = new Game();

        game.Start("Bowler");

        game.Roll(8);
        Assert.That(game.Roll(2).Spare, Is.True);
    }

    [Test]
    public void Should_be_able_to_throw_an_open()
    {
        var game = new Game();

        game.Start("Bowler");

        game.Roll(2);
        Assert.That(game.Roll(2).Open, Is.True);
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

        var pinfall = game.Roll(10);

        Assert.That(pinfall.GameFinished, Is.True);
        Assert.That(pinfall.Score, Is.EqualTo(300));
    }

    [Test]
    public void Should_be_able_to_throw_a_bonus_ball_from_a_spare()
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
        game.Roll(5);
        game.Roll(5);

        var pinfall = game.Roll(10);

        Assert.That(pinfall.GameFinished, Is.True);
        Assert.That(pinfall.Score, Is.EqualTo(275));
    }

    [Test]
    public void Should_be_able_to_throw_5_with_spares()
    {
        var game = new Game();

        game.Start("Bowler");

        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);
        game.Roll(5);

        var pinfall = game.Roll(5);

        Assert.That(pinfall.GameFinished, Is.True);
        Assert.That(pinfall.Score, Is.EqualTo(150));
    }
}
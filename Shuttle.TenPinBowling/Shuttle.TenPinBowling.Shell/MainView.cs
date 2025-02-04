﻿using System;
using System.Windows.Forms;
using Shuttle.Core.Data;

namespace Shuttle.TenPinBowling.Shell;

public partial class MainView : Form, IMainView
{
    private IModel _model = new Model();
    private IMainPresenter _presenter = null!;

    public MainView()
    {
        InitializeComponent();
    }

    public void Assign(IMainPresenter presenter, IModel model)
    {
        _presenter = presenter;
        _model = model;

        WireEvents(model);
    }

    public void ShowMessage(string message)
    {
        MessageBox.Show(message, @"Message", MessageBoxButtons.OK);
    }

    public void GameFinished()
    {
        HidePinButtons(-1);
    }

    private void FrameScored(object? sender, EventArgs e)
    {
        foreach (var model in _model.FrameScores)
        {
            var frame = GetFrame(model.Frame);

            frame.Roll1(model.Roll1PinsDisplay);

            if (model.Roll2Pins.HasValue)
            {
                frame.Roll2(model.Roll2PinsDisplay);
            }

            if (model.Roll3Pins.HasValue)
            {
                frame.Roll3(model.Roll3PinsDisplay);
            }

            frame.Score(model.Score);
        }

        ScoreDisplay.Text = _model.Score.ToString();

        ShowStandingPins();
    }

    private void GameAdded(object? sender, GameAddedEventArgs e)
    {
        Games.Items.Add($"{e.Bowler} ({e.DateStarted:yyyy-MM-dd hh:mm})")
            .Tag = e.Id;
    }

    private async void Games_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (Games.SelectedItems.Count == 0)
        {
            return;
        }

        using var scope = new DatabaseContextScope();

        await _presenter.SelectGameAsync((Guid)(Games.SelectedItems[0].Tag ?? Guid.Empty));
    }

    private void GameStarted(object? sender, EventArgs e)
    {
        Bowler.Text = _model.Bowler;

        for (var i = 1; i < 11; i++)
        {
            GetFrame(i).Reset();
        }

        ShowStandingPins();
        ScoreDisplay.Text = string.Empty;
    }

    private Frame GetFrame(int frame)
    {
        var name = string.Concat("Frame", frame);

        foreach (Control control in Controls)
        {
            if (control.Name.Equals(name))
            {
                return (Frame)control;
            }
        }

        throw new ApplicationException($"Could not find a frame control for frame number '{frame}'.");
    }

    private Button GetPinButton(int pin)
    {
        var name = $"Pin{pin}Button";

        foreach (Control control in Controls)
        {
            if (control.Name.Equals(name))
            {
                return (Button)control;
            }
        }

        throw new ApplicationException($"Could not find a pin button control for pin number '{pin}'.");
    }

    private void HidePinButtons(int from)
    {
        for (var i = 0; i < 11; i++)
        {
            GetPinButton(i).Visible = i <= from;
        }
    }

    private async void PinKnockDownButton_Click(object? sender, EventArgs e)
    {
        if (sender == null)
        {
            return;
        }

        await _presenter.RollAsync(int.Parse(((Button)sender).Text));
    }

    private void ShowStandingPins()
    {
        HidePinButtons(_model.StandingPins);
    }

    private async void StartGameButton_Click(object? sender, EventArgs e)
    {
        await _presenter.StartGameAsync(Bowler.Text);
    }

    private void WireEvents(IModel model)
    {
        model.GameStarted += GameStarted;
        model.GameAdded += GameAdded;
        model.FrameScored += FrameScored;
    }
}
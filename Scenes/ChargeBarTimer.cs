using Godot;

namespace MonsterHunterIdle;

public partial class ChargeBarTimer : TextureRect
{
    [Signal]
    public delegate void ChargingFinishedEventHandler();

    [Export]
    private TextureProgressBar _textureProgressBar;

    [Export]
    private Timer _timer;

    public override void _Ready()
    {
        _timer.Timeout += () =>
        {
            EmitSignal(SignalName.ChargingFinished);
            QueueFree();
        };

        Position = GetLocalMousePosition() - (Size / 2);

        _timer.Start();
    }

    public override void _Process(double delta)
	{
		if (_timer.TimeLeft == 0) return;

		_textureProgressBar.Value = _timer.TimeLeft;
	}

    public void SetTimeThreshold(float timeThreshold)
    {
        _textureProgressBar.MaxValue = timeThreshold;
        _timer.WaitTime = timeThreshold;
    }
}

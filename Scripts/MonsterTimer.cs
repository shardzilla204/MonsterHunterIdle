using Godot;

namespace MonsterHunterIdle;

public partial class MonsterTimer : TextureRect
{
	[Export]
	private TextureProgressBar _progressBar;

	[Export]
	private Timer _timer;

	public override void _Ready()
	{
		_timer.Timeout += () => MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.MonsterLeft);
	}

	public override void _Process(double delta)
	{
		if (_timer.TimeLeft == 0) return;

		_progressBar.Value = _timer.TimeLeft;
	}

	public void Start()
	{
		_timer.Start();
		Visible = true;
		_progressBar.MaxValue = _timer.WaitTime;
		_progressBar.Value = _timer.WaitTime;
	}

	public void Stop()
	{
		_timer.Stop();
		Visible = false;
	}
}

using Godot;

namespace MonsterHunterIdle;

public partial class HunterProgress : NinePatchRect
{
	[Export]
	private TextureProgressBar _progressBar;

	[Export]
	private Label _progressLabel;

	public void Update()
	{
		_progressBar.MaxValue = Hunter.PointsRequired;
		_progressBar.Value = Hunter.Points;
		_progressLabel.Text = $"{_progressBar.Value} / {_progressBar.MaxValue}";
	}
}

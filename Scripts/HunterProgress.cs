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
		_progressBar.MaxValue = MonsterHunterIdle.HunterManager.Hunter.PointsRequired;
		_progressBar.Value = MonsterHunterIdle.HunterManager.Hunter.Points;
		_progressLabel.Text = $"{_progressBar.Value} / {_progressBar.MaxValue}";
	}
}

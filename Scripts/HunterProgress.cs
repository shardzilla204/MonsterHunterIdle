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
		_progressBar.Value = GameManager.Instance.Player.HunterProgress;
		_progressBar.MaxValue = GameManager.Instance.Player.MaxHunterProgress;
		_progressLabel.Text = $"{_progressBar.Value} / {_progressBar.MaxValue}";
	}
}

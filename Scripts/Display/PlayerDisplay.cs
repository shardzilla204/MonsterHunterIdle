using Godot;

namespace MonsterHunterIdle;

public partial class PlayerDisplay : NinePatchRect
{
	[Export]
	private Label _hunterRankLabel;

	[Export]
	private HunterProgress _hunterProgress;

	[Export]
	private Label _zennyLabel;

	public override void _Ready()
	{
		GameManager.Instance.LeveledUp += Update;
		TreeExited += () => GameManager.Instance.LeveledUp -= Update;

		Update();
	}

	private void Update()
	{
		_hunterRankLabel.Text = $"Hunter Rank: {GameManager.Instance.Player.HunterRank}";
		_hunterProgress.Update();
		_zennyLabel.Text = $"{GameManager.Instance.Player.Zenny}";
	}
}

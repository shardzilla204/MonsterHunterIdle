using Godot;

namespace MonsterHunterIdle;

public partial class HunterInterface : NinePatchRect
{
	[Export]
	private Label _hunterRankLabel;

	[Export]
	private HunterProgress _hunterProgress;

	[Export]
	private Label _zennyLabel;

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.HunterLeveledUp -= OnHunterLeveledUp;
    }

    public override void _EnterTree()
    {
		MonsterHunterIdle.Signals.HunterLeveledUp += OnHunterLeveledUp;
    }

	public override void _Ready()
	{
		// Set initially
		OnHunterLeveledUp();
	}

	private void OnHunterLeveledUp()
	{
		_hunterRankLabel.Text = $"Hunter Rank: {MonsterHunterIdle.HunterManager.Hunter.Rank}";
		_hunterProgress.Update();
		_zennyLabel.Text = $"{MonsterHunterIdle.HunterManager.Hunter.Zenny}";
	}
}

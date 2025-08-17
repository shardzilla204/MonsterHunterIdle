using Godot;

namespace MonsterHunterIdle;

public partial class InterfaceController : Container
{
	[Export]
	private TextureRect _gatherIcon;

	[Export]
	private TextureProgressBar _hunterProgressBar;

    public override void _EnterTree()
    {
		MonsterHunterIdle.Signals.LocaleChanged += () => _gatherIcon.Texture = MonsterHunterIdle.LocaleManager.Locale.GatherIcon;
		MonsterHunterIdle.Signals.HunterPointsChanged += OnHunterPointsChanged;
    }


	public override void _Ready()
	{
		// Set initially
		OnHunterPointsChanged();
	}

	private void OnHunterPointsChanged()
	{
		_hunterProgressBar.MaxValue = MonsterHunterIdle.HunterManager.Hunter.PointsRequired;
		_hunterProgressBar.Value = MonsterHunterIdle.HunterManager.Hunter.Points;
	}
}

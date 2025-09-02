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
		MonsterHunterIdle.Signals.LocaleChanged += () => _gatherIcon.Texture = LocaleManager.Locale.GatherIcon;
		MonsterHunterIdle.Signals.HunterPointsChanged += OnHunterPointsChanged;
		MonsterHunterIdle.Signals.GameDeleted += OnHunterPointsChanged;
	}

	public override void _Ready()
	{
		// Set initially
		OnHunterPointsChanged();
	}

	// * START - Signal Methods
	private void OnHunterPointsChanged()
	{
		_hunterProgressBar.MaxValue = Hunter.PointsRequired;
		_hunterProgressBar.Value = Hunter.Points;
	}
	// * END - Signal Methods
}

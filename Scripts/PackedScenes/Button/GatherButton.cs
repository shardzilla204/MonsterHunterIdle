using Godot;

namespace MonsterHunterIdle;

public partial class GatherButton : CustomButton
{
	[Export]
	private TextureProgressBar _gatherProgress;

	[Export]
	private TextureRect _gatherIcon;

	private bool _isDown = false;
	private const float _ProgressValue = 0.25f;

	public override void _ExitTree()
	{
		MonsterHunterIdle.Signals.LocaleChanged -= OnLocaleChanged;
	}

    public override void _EnterTree()
    {
		base._EnterTree();
		
		MonsterHunterIdle.Signals.LocaleChanged += OnLocaleChanged;
    }

	public override void _Ready()
	{
		base._Ready();

		ButtonDown += () => _isDown = true;
		ButtonUp += () => _isDown = false;

		// Set the locale icon initially
		OnLocaleChanged();
	}

	// Checks the progress
	public override void _Process(double delta)
	{
		_gatherProgress.Value += _isDown ? _ProgressValue : -_ProgressValue;

		if (_gatherProgress.Value < _gatherProgress.MaxValue) return;

		_gatherProgress.Value = 0;

		LocaleMaterial localeMaterial = LocaleManager.GetLocaleMaterial();
		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.LocaleMaterialAdded, localeMaterial);

		HunterManager.AddHunterPoints(1);
	}

	// * START - Signal Methods
	private void OnLocaleChanged()
	{
		LocaleType localeType = LocaleManager.Locale.Type;
		_gatherIcon.Texture = LocaleManager.GetGatherIcon(localeType);
	}
	// * END - Signal Methods
}

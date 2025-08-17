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

	public override void _Ready()
	{
		base._Ready();
		MonsterHunterIdle.Signals.LocaleChanged += OnLocaleChanged;

		ButtonDown += () => _isDown = true;
		ButtonUp += () => _isDown = false;

		// Set the locale icon initially
		OnLocaleChanged();
	}

	public override void _Process(double delta)
	{
		CheckProgress();
	}

	private void CheckProgress()
	{
		_gatherProgress.Value += _isDown ? _ProgressValue : -_ProgressValue;

		if (_gatherProgress.Value < _gatherProgress.MaxValue) return;

		_gatherProgress.Value = 0;

		LocaleMaterial localeMaterial = MonsterHunterIdle.LocaleManager.GetLocaleMaterial();
		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.LocaleMaterialAdded, localeMaterial);

		MonsterHunterIdle.HunterManager.AddHunterPoints(1);
	}

	private void OnLocaleChanged()
	{
		LocaleType localeType = MonsterHunterIdle.LocaleManager.Locale.Type;
		_gatherIcon.Texture = MonsterHunterIdle.LocaleManager.GetGatherIcon(localeType);
	}
}

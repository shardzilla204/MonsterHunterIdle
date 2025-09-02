using Godot;

namespace MonsterHunterIdle;

public partial class CycleButton : CustomButton
{
	[Export]
	private bool _isClockwise = false;

	[Export]
	private TextureRect _localeIcon;

	public override void _Ready()
	{
		base._Ready();

		Pressed += OnCycleButtonPressed;
		MonsterHunterIdle.Signals.LocaleChanged += OnLocaleChanged;

		// Set the locale icon initially
		OnLocaleChanged();
	}
	// * START - Signal Methods
	private void OnLocaleChanged()
	{
		Locale locale = _isClockwise ? LocaleManager.GetNextLocale() : LocaleManager.GetPreviousLocale();
		_localeIcon.Texture = LocaleManager.GetLocaleIcon(locale.Type);
	}

	private async void OnCycleButtonPressed()
	{
		bool result = await LocaleManager.CycleLocale(_isClockwise);
		if (!result) return;

		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.LocaleChanged);
	}
	// * END - Signal Methods
}

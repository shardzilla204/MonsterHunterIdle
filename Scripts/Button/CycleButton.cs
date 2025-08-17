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

		Pressed += CycleLocale;
		MonsterHunterIdle.Signals.LocaleChanged += OnLocaleChanged;

		// Set the locale icon initially
		OnLocaleChanged();
	}

	private void OnLocaleChanged()
	{
		Locale locale = _isClockwise ? MonsterHunterIdle.LocaleManager.GetNextLocale() : MonsterHunterIdle.LocaleManager.GetPreviousLocale();
		_localeIcon.Texture = MonsterHunterIdle.LocaleManager.GetLocaleIcon(locale.Type);
	}

	private async void CycleLocale()
	{
		bool result = await MonsterHunterIdle.LocaleManager.CycleLocale(_isClockwise);
		if (!result) return;

		MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.LocaleChanged);
	}
}

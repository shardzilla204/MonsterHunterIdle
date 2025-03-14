using Godot;

namespace MonsterHunterIdle;

public enum Display
{
	Settings,
	CollectionLog,
	ItemBox,
	Loadout,
	Player,
	Palico,
	Smithy
}

public partial class InterfaceController : Container
{
	[Export]
	private Button _settingsButton;
	
	[Export]
	private Button _gatherButton;

	[Export]
	private TextureRect _gatherIcon;

	[Export]
	private Button _itemBoxButton;

	[Export]
	private Button _smithyButton;

	[Export]
	private Button _loadoutButton;

	[Export]
	private Button _playerButton;

	[Export]
	private Button _palicoButton;

	public override void _Ready()
	{
		// _settingsButton.Pressed += () => ChangeDisplay(Display.Settings);
		_gatherButton.Pressed += () => ChangeDisplay(Display.CollectionLog);
		_itemBoxButton.Pressed += () => ChangeDisplay(Display.ItemBox);
		_smithyButton.Pressed += () => ChangeDisplay(Display.Smithy);
		// _loadoutButton.Pressed += () => ChangeDisplay(Display.Loadout);
		_playerButton.Pressed += () => ChangeDisplay(Display.Player);
		_palicoButton.Pressed += () => ChangeDisplay(Display.Palico);

		MonsterHunterIdle.Signals.Changed.Instance.Updated += () => _gatherIcon.Texture = BiomeManager.Instance.Biome.GatherIcon;
	}

	private void ChangeDisplay(Display display)
	{
		GameManager.Instance.ChangeDisplay(display);
	}
}

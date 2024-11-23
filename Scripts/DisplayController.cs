using Godot;

namespace MonsterHunterIdle;

public enum Display
{
	Settings,
	CollectionLog,
	ItemBox,
	Loadout,
	Player
}

public partial class DisplayController : Container
{
	private static DisplayController _instance;
	public static DisplayController Instance
	{
		get => _instance;
		private set 
		{
			if (_instance == null)
			{
				_instance = value;
			}
			else if (_instance != value)
			{
				GD.PrintRich($"{nameof(DisplayController)} already exists");
			}
		}
	}

	[Export]
	private Button _settingsButton;
	
	[Export]
	private Button _gatherButton;

	[Export]
	private TextureRect _gatherIcon;

	[Export]
	private Button _itemBoxButton;

	[Export]
	private Button _loadoutButton;

	[Export]
	private Button _playerButton;

	public override void _Ready()
	{
		Instance = this;
		// _settingsButton.Pressed += () => ChangeDisplay(Display.Settings);
		_gatherButton.Pressed += () => ChangeDisplay(Display.CollectionLog);
		_itemBoxButton.Pressed += () => ChangeDisplay(Display.ItemBox);
		// _loadoutButton.Pressed += () => ChangeDisplay(Display.Loadout);
		_playerButton.Pressed += () => ChangeDisplay(Display.Player);

		BiomeManager.Instance.Updated += () => _gatherIcon.Texture = BiomeManager.Instance.Biome.GatherIcon;
	}

	private void ChangeDisplay(Display display)
	{
		GameManager.Instance.ChangeDisplay(display);
	}
}

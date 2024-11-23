using Godot;

namespace MonsterHunterIdle;

public partial class Game : Node
{
	private static Game _instance;
	public static Game Instance
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
				GD.PrintRich($"{nameof(Game)} already exists");
			}
		}
	}

	[Export]
	public Container MainContainer;

	public override void _Ready()
	{
		Instance = this;
		GameManager.Instance.ClearDisplays();
		GameManager.Instance.ChangeDisplay(Display.CollectionLog);
	}
}

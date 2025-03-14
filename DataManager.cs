using Godot;

namespace MonsterHunterIdle;

public partial class DataManager : Node
{
	private static DataManager _instance;
	public static DataManager Instance
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
				GD.PrintRich($"{nameof(GameManager)} already exists");
			}
		}
	}

	[Export]
	public PlayerDataHolder Player;

	[Export]
	public PalicoDataHolder Palico;

    public override void _Ready()
    {
        Instance = this;
    }
}

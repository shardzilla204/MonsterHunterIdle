using System.Collections.Generic;
using System.Linq;
using Godot;

namespace MonsterHunterIdle;

public enum RarityLevel
{
	One = 1000,
	Two = 500,
	Three = 250,
	Four = 125,
	Five = 50,
	Six = 25
	// One = 5000,
	// Two = 2500,
	// Three = 1000,
	// Four = 200,
	// Five = 50,
	// Six = 10
}

public enum ElementType
{
	Fire,
	Water,
	Thunder,
	Ice,
	Dragon
}

public enum AbnormalStatusType
{
	Poison,
	Paralysis,
	Sleep,
	Stun,
	BlastBlight,
	BubbleBlight,
	HellfireBlight,
}

public partial class GameManager : Node
{
	[Signal]
	public delegate void LeveledUpEventHandler();

	private static GameManager _instance;
	public static GameManager Instance
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
	private PackedScene _collectionLogDisplayScene;

	[Export]
	private PackedScene _itemBoxDisplayScene;

	[Export]
	private PackedScene _monsterDisplayScene;

	[Export]
	private PackedScene _playerDisplayScene;

	public ItemBoxData ItemBox = new ItemBoxData();

	public PlayerData Player = new PlayerData();

    public override void _Ready()
    {
        Instance = this;
		Player.SetValues();
    }

	public void ShowMaterials()
	{
		GD.Print("Item Box: ");
		for (int i = 0; i < ItemBox.Materials.Count; i++)
		{
			GD.PrintRich($"\t[color=orange]{ItemBox.Materials[i].Name}[/color]");
		}
		GD.Print("");
	}

	public void ChangeDisplay(Display displayType)
	{
		ClearDisplays();

		Game.Instance.MainContainer.AddChild(displayType switch
		{
			Display.CollectionLog => CreateDisplay(_collectionLogDisplayScene),
			Display.ItemBox => CreateDisplay(_itemBoxDisplayScene),
			Display.Player => CreateDisplay(_playerDisplayScene),
			_ => CreateDisplay(_collectionLogDisplayScene),
		});

		if (displayType == Display.Settings) return;
		
		Game.Instance.MainContainer.AddChild(CreateDisplay(_monsterDisplayScene));
	}

	public void ClearDisplays()
	{
		List<Node> currentDisplays = Game.Instance.MainContainer.GetChildren().ToList();
		foreach (Node display in currentDisplays)
		{
			Game.Instance.MainContainer.RemoveChild(display);
			display.QueueFree();
		}
	}

	private dynamic CreateDisplay(PackedScene packedScene)
	{
		return packedScene.Instantiate<dynamic>();
	}

	// TODO: Save original monster data and copy from it when there is a monster encounter and store its current data

	public void GetRewards(int progressAmount, int zennyAmount)
	{
		Player.Zenny += zennyAmount;

		Player.HunterProgress += progressAmount;
		CheckPlayerProgress();
	}

	private void AddPlayerProgress(int progress)
	{
		Player.HunterProgress += progress;
	}

	private void CheckPlayerProgress()
	{
		if (Player.HunterProgress < Player.MaxHunterProgress) return;

		Player.HunterRank++;
		Player.HunterProgress -= Player.MaxHunterProgress;
		GetMaxHunterProgress();

		EmitSignal(SignalName.LeveledUp);
	}

	private void GetMaxHunterProgress()
	{
		if (Player.HunterRank < 100) Player.MaxHunterProgress += 100;
	}
}

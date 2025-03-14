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

public partial class GameManager : Node
{
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
	public ItemBoxData ItemBox = new ItemBoxData();

	[Export]
	public PlayerData Player = new PlayerData(1, 250);

    public override void _Ready()
    {
        Instance = this;
    }

	public void ChangeDisplay(Display displayType)
	{
		ClearDisplays();

		Game.Instance.MainContainer.AddChild(displayType switch
		{
			Display.CollectionLog => MonsterHunterIdle.PackedScenes.GetCollectionLogDisplay(),
			Display.ItemBox => MonsterHunterIdle.PackedScenes.GetItemBoxDisplay(),
			Display.Smithy => MonsterHunterIdle.PackedScenes.GetSmithyDisplay(),
			Display.Player => MonsterHunterIdle.PackedScenes.GetPlayerDisplay(),
			Display.Palico => MonsterHunterIdle.PackedScenes.GetPalicoDisplay(),
			_ => MonsterHunterIdle.PackedScenes.GetCollectionLogDisplay(),
		});

		if (displayType == Display.Settings) return;
		
		Game.Instance.MainContainer.AddChild(MonsterHunterIdle.PackedScenes.GetMonsterDisplay());
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
	}

	private void GetMaxHunterProgress()
	{
		if (Player.HunterRank < 100) Player.MaxHunterProgress += 100;
	}
}

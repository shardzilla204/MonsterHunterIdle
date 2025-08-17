using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;

namespace MonsterHunterIdle;

public partial class GameManager : Node
{
	private string _gameFilePath = "user://savegame.sav";

	public override void _EnterTree()
	{
		MonsterHunterIdle.GameManager = this;

		MonsterHunterIdle.Signals.GameSaved += SaveGame;
		MonsterHunterIdle.Signals.GameLoaded += LoadGame;
		MonsterHunterIdle.Signals.GameDeleted += DeleteGame;

		GetWindow().CloseRequested += SaveGame;
	}

	public override void _Ready()
	{
		if (FileAccess.FileExists(_gameFilePath))
		{
			LoadGame();
		}
		else
		{
			DeleteGame(); // Set up save file
		}
	}

	private GC.Dictionary<string, Variant> GetData()
	{
		return new GC.Dictionary<string, Variant>()
		{
			{ "Hunter", MonsterHunterIdle.HunterManager.GetData() },
			{ "ItemBox", MonsterHunterIdle.ItemBox.GetData() }
		};
	}

	private void SetData(GC.Dictionary<string, Variant> gameData)
	{
		try
		{
			GC.Dictionary<string, Variant> hunterData = gameData["Hunter"].As<GC.Dictionary<string, Variant>>();
			MonsterHunterIdle.HunterManager.SetData(hunterData);

			GC.Dictionary<string, Variant> itemBoxData = gameData["ItemBox"].As<GC.Dictionary<string, Variant>>();
			MonsterHunterIdle.ItemBox.SetData(itemBoxData);

			// GC.Dictionary<string, Variant> autoPicklesData = gameData["Auto Pickles"].As<GC.Dictionary<string, Variant>>();
			// PickleClicker.AutoPickleManager.SetData(autoPicklesData);

			// GC.Dictionary<string, Variant> upgradePicklesData = gameData["Upgrade Pickles"].As<GC.Dictionary<string, Variant>>();
			// PickleClicker.UpgradePickleManager.SetData(upgradePicklesData);

			// GC.Dictionary<string, Variant> poglinEnemiesData = gameData["Poglin Enemies"].As<GC.Dictionary<string, Variant>>();
			// PickleClicker.PoglinEnemyManager.SetData(poglinEnemiesData);
		}
		catch (KeyNotFoundException)
		{
			GD.PrintErr("Couldn't set data. Saving game");
			SaveGame();
		}
	}

	public void SaveGame()
	{
		using FileAccess gameFile = FileAccess.Open(_gameFilePath, FileAccess.ModeFlags.Write);
		string jsonString = Json.Stringify(GetData(), "\t");

		if (jsonString == "") return;

		gameFile.StoreLine(jsonString);

		string saveSuccessMessage = "Game File Successfully Saved";
		if (PrintRich.AreFilePathsVisible)
		{
			saveSuccessMessage += $" At {gameFile.GetPathAbsolute()}";
		}
		PrintRich.PrintLine(TextColor.Green, saveSuccessMessage);
	}

	public void LoadGame()
	{
		using FileAccess gameFile = FileAccess.Open(_gameFilePath, FileAccess.ModeFlags.Read);
		string jsonString = gameFile.GetAsText();

		if (gameFile.GetLength() == 0)
		{
			GD.PrintErr("Game File Is Empty");
			return;
		}

		Json json = new Json();

		Error result = json.Parse(jsonString);

		if (result != Error.Ok) return;

		GC.Dictionary<string, Variant> gameData = new GC.Dictionary<string, Variant>((GC.Dictionary) json.Data);
		SetData(gameData);

		string loadSuccessMessage = "Game File Successfully Loaded";
		if (PrintRich.AreFilePathsVisible)
		{
			loadSuccessMessage += $" At {gameFile.GetPathAbsolute()}";
		}
		PrintRich.PrintLine(TextColor.Green, loadSuccessMessage);
	}

	private void DeleteGame()
	{
		MonsterHunterIdle.HunterManager.DeleteData();
		MonsterHunterIdle.ItemBox.DeleteData();

		SaveGame();
	}
}

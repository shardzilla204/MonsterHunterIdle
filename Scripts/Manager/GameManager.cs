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
			{ "ItemBox", MonsterHunterIdle.ItemBox.GetData() },
			{ "Equipment", MonsterHunterIdle.EquipmentManager.GetData() },
			{ "Palicos", MonsterHunterIdle.PalicoManager.GetData() }
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

			GC.Dictionary<string, Variant> equipmentData = gameData["Equipment"].As<GC.Dictionary<string, Variant>>();
			MonsterHunterIdle.EquipmentManager.SetData(equipmentData);

			GC.Array<GC.Dictionary<string, Variant>> palicosData = gameData["Palicos"].As<GC.Array<GC.Dictionary<string, Variant>>>();
			MonsterHunterIdle.PalicoManager.SetData(palicosData);

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

	// ! Important !
	/// Erase equipment first as added starting equipment from deleting hunter data will also be erased 
	private void DeleteGame()
	{
		MonsterHunterIdle.EquipmentManager.DeleteData();
		MonsterHunterIdle.ItemBox.DeleteData();
		MonsterHunterIdle.HunterManager.DeleteData();
		MonsterHunterIdle.PalicoManager.DeleteData();

		SaveGame();
	}
}

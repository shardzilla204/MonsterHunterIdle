using Godot;
using GC = Godot.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace MonsterHunterIdle;

public partial class GameManager : Node
{
	private static string _gameFilePath = "user://savegame.sav";

	public override void _EnterTree()
	{
		MonsterHunterIdle.Signals.GameSaved += OnGameSaved;
		MonsterHunterIdle.Signals.GameLoaded += OnGameLoaded;
		MonsterHunterIdle.Signals.GameDeleted += OnGameDeleted;
		MonsterHunterIdle.Signals.GameQuit += () =>
		{
			OnGameSaved();
			GetTree().Quit();
		};

		GetWindow().CloseRequested += OnGameSaved;
	}

	public override void _Ready()
	{
		if (FileAccess.FileExists(_gameFilePath))
		{
			OnGameLoaded();
		}
		else
		{
			OnGameDeleted(); // Set up save file
		}
	}

	// * START - Signal Methods
	public static void OnGameSaved()
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
		PrintRich.PrintSuccess(saveSuccessMessage);
	}

	public static void OnGameLoaded()
	{
		using FileAccess gameFile = FileAccess.Open(_gameFilePath, FileAccess.ModeFlags.Read);
		string jsonString = gameFile.GetAsText();

		if (gameFile.GetLength() == 0)
		{
			string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Game File Is Empty";
            PrintRich.PrintError(className, message);

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
		PrintRich.PrintSuccess(loadSuccessMessage);
	}

	// ! Important !
	/// Erase equipment first as added starting equipment from deleting hunter data will also be erased 
	private static void OnGameDeleted()
	{
		ItemBox.DeleteData();
		HunterManager.DeleteData();
		PalicoManager.DeleteData();
		EquipmentManager.DeleteData();
		PalicoEquipmentManager.DeleteData();

		OnGameSaved();
	}
	// * END - Signal Methods

	private static GC.Dictionary<string, Variant> GetData()
	{
		return new GC.Dictionary<string, Variant>()
		{
			{ "Hunter", HunterManager.GetData() },
			{ "ItemBox", ItemBox.GetData() },
			{ "Equipment", EquipmentManager.GetData() },
			{ "Palicos", PalicoManager.GetData() },
			{ "PalicoEquipment", PalicoEquipmentManager.GetData() },
		};
	}

	private static void SetData(GC.Dictionary<string, Variant> gameData)
	{
		try
		{
			GC.Dictionary<string, Variant> hunterData = gameData["Hunter"].As<GC.Dictionary<string, Variant>>();
			HunterManager.SetData(hunterData);

			GC.Dictionary<string, Variant> itemBoxData = gameData["ItemBox"].As<GC.Dictionary<string, Variant>>();
			ItemBox.SetData(itemBoxData);

			GC.Dictionary<string, Variant> equipmentData = gameData["Equipment"].As<GC.Dictionary<string, Variant>>();
			EquipmentManager.SetData(equipmentData);

			GC.Array<GC.Dictionary<string, Variant>> palicosData = gameData["Palicos"].As<GC.Array<GC.Dictionary<string, Variant>>>();
			PalicoManager.SetData(palicosData);

			GC.Dictionary<string, Variant> palicoEquipmentData = gameData["PalicoEquipment"].As<GC.Dictionary<string, Variant>>();
			PalicoEquipmentManager.SetData(palicoEquipmentData);
		}
		catch (KeyNotFoundException)
		{
			string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Couldn't Load Data";
            string result = $"Saving Game";
            PrintRich.PrintError(className, message, result);
			
			OnGameSaved();
		}
	}
}

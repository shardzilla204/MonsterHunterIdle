using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public enum ElementType
{
	Fire,
	Water,
	Thunder,
	Ice,
	Dragon
}

public enum AbnormalStatType
{
	Poison,
	Paralysis,
	Sleep,
	Stun,
	BlastBlight,
	BubbleBlight,
	HellfireBlight,
}

public enum StatType
{
	Attack,
	Defense,
	Affinity,
	Water,
	Fire,
	Thunder,
	Ice,
	Dragon,
	Health
}

/*
	TODO: Add Palico equipment data
	// TODO: Add "offline" play
	// TODO: Add filter when crafting equipment
	// TODO: Add Ability To Create & Use Equipment
	// TODO: Show interface to ask if the user wants to change to the equipment that's just been crafted
	// TODO: Add ability to swap crafted equipment
	// TODO: Rework Palico mechanic so that it will attack if there's a monster. If not then just gather materials
*/

// ! Important Note !
/// Create a new object for equipment to act as "null" | <see cref="Hunter"/> | e.g. Hunter.Head = new Armor(ArmorCategory.Head);

public partial class MonsterHunterIdle : Node
{
	[Export]
	private InterfaceType _startingInterface;

	[Export(PropertyHint.Range, "1, 6, 0.1")]
	private float _offlineThresholdMult = 2f;

	public static HunterManager HunterManager;
	public static PalicoManager PalicoManager;
	public static LocaleManager LocaleManager;
	public static MonsterManager MonsterManager;
	public static EquipmentManager EquipmentManager;
	public static GameManager GameManager;
	public static ItemBox ItemBox;
	public static OfflineProgress OfflineProgress;

	public static Signals Signals = new Signals();
	public static PackedScenes PackedScenes;

	public static InterfaceType StartingInterface;
	public static float OfflineThresholdMult;

	public override void _EnterTree()
	{
		StartingInterface = _startingInterface;
		OfflineThresholdMult = _offlineThresholdMult;
	}

	public static Texture2D GetStatIcon(StatType statType)
	{
		string filePath = $"res://Assets/Images/Stat/{statType}Icon.png";
		return GetTexture(filePath);
	}

	public static Texture2D GetMonsterIcon(string monsterName)
	{
		monsterName = monsterName.Replace(" ", "");
		monsterName = monsterName.Replace("-", "");
		string filePath = $"res://Assets/Images/Monster/Icon/{monsterName}Icon.png";
		return GetTexture(filePath);
	}

	public static Texture2D GetMonsterRender(string monsterName)
	{
		monsterName = monsterName.Replace(" ", "");
		monsterName = monsterName.Replace("-", "");
		string filePath = $"res://Assets/Images/Monster/Render/{monsterName}Render.png";
		return GetTexture(filePath);
	}

	public static Texture2D GetEquipmentIcon(Equipment equipment)
	{
		string filePath = "";
		if (equipment is Weapon)
		{
			filePath = "res://Assets/Images/Icon/EquipmentIcon.png";
		}
		else if (equipment is Armor armor)
		{
			filePath = $"res://Assets/Images/Icon/{armor.Category}EquipmentIcon.png";
		}
		return GetTexture(filePath);
	}

	public static Texture2D GetTexture(string filePath)
	{
		return ResourceLoader.Load<Texture2D>(filePath);
	}

	public static int GetMaterialWeight(int rarity) => rarity switch
	{
		1 => 7500,
		2 => 3000,
		3 => 1650,
		4 => 650,
		5 => 400,
		6 => 200,
		7 => 100,
		8 => 75,
		9 => 30,
		10 => 10,
		_ => throw new ArgumentOutOfRangeException("Rarity", $"Not expected rarity value: {rarity}")
	};

	public static Texture2D GetMaterialIcon(Material material)
	{
		string fileName = $"res://Assets/Images/Material/{material.Type}{material.Color}.png";
		return ResourceLoader.Load<Texture2D>(fileName);
	}

	public void GetRewards(int progressAmount, int zennyAmount)
	{
		HunterManager.Hunter.Zenny += zennyAmount;

		HunterManager.Hunter.Points += progressAmount;
		CheckHunterProgress();
	}

	private void AddHunterProgress(int progress)
	{
		HunterManager.Hunter.Points += progress;
	}

	private void CheckHunterProgress()
	{
		if (HunterManager.Hunter.Points < HunterManager.Hunter.PointsRequired) return;

		HunterManager.Hunter.Rank++;
		HunterManager.Hunter.Points -= HunterManager.Hunter.PointsRequired;
		GetMaxHunterProgress();
	}

	private void GetMaxHunterProgress()
	{
		if (HunterManager.Hunter.Rank < 100) HunterManager.Hunter.PointsRequired += 100;
	}

	public static Variant LoadFile(string fileName, string folderName = "")
	{
		folderName = folderName != "" ? $"{folderName}/" : "";
		string filePath = $"res://JSON/{folderName}{fileName}.json";

		Json json = new Json();

		try
		{
			using FileAccess fileAccess = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
			string jsonString = fileAccess.GetAsText();

			if (json.Parse(jsonString) != Error.Ok) throw new Exception($"Couldn't Parse {fileName}");

			PrintRich.PrintSuccess(fileName);

			GC.Dictionary<string, Variant> dictionaries = (GC.Dictionary<string, Variant>)json.Data;
			return dictionaries;
		}
		catch (Exception exception)
		{
			if (exception.Message.Trim() != "")
			{
				GD.PrintErr(exception.Message);
			}
			else
			{
				GD.PrintErr(exception);
			}
			return default;
		}
	}

	public static Material FindMaterial(string materialName)
	{
		Material material = LocaleManager.Materials.Find(material => material.Name == materialName);
		if (material != null)
		{
			return material;
		}
		else
		{
			material = MonsterManager.Materials.Find(material => material.Name == materialName);
		}

		if (material == null) GD.PrintErr("Couldn't Find Material. Returning Null");

		return material;
	}

	public static Material GetRandomMaterial()
	{
		RandomNumberGenerator RNG = new RandomNumberGenerator();

		// Add all materials from every locale
		List<Material> materials = [.. LocaleManager.Materials];

		// Get a random monster from any locale, get their materials and add them to the list
		int localeCount = LocaleManager.LocaleQueue.Count - 1;
		int randomLocaleIndex = RNG.RandiRange(0, localeCount);
		Locale locale = LocaleManager.LocaleQueue[randomLocaleIndex];
		Monster monster = MonsterManager.GetRandomMonster(locale);
		List<MonsterMaterial> monsterMaterials = MonsterManager.GetMonsterMaterials(monster);
		materials.AddRange(monsterMaterials);

		int index = RNG.RandiRange(0, materials.Count - 1);
		return materials[index];
	}

	// Add spaces in between capital letters
	public static string AddSpacing(string words)
	{
		return string.Concat(words.Select(letter => char.IsUpper(letter) ? " " + letter : letter.ToString())).TrimStart(' ');
	}
}

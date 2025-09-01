using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public enum SpecialType
{
	None = -1,
	Fire,
	Water,
	Thunder,
	Ice,
	Dragon,
	Poison,
	Paralysis,
	Sleep,
	Stun,
	Blastblight
}

public enum StatType
{
	Attack,
	Defense,
	Affinity,
	Health
}

/*
	TODO: Fix swapping out equipment
	TODO: Add weapons:
	//	Great Sword
	//	Long Sword

	// TODO: Add Palico equipment data
	// TODO: Show the recipe of the first item in list from palico equipment
	// TODO: Reassign equipment index when upgrading palico equipment
	// TODO: When exiting the recipe interface, also remove the palico craft option interface
	// TODO: Fix bug where you can make as much of the palico equipment as you want

	// TODO: Add attacking charge bar
	// TODO: Add monster weakness damage
	// TODO: Add equipment status element
	// TODO: Have filter support multiple groups
	// TODO: Fix monster level not being filtered with hunter rank
		
	// TODO: Add equipment status element
	// TODO: Just have two names and add on the Roman numeral
	// TODO: Add monsters
		// Barroth
		// Great Girros
		// Tobi-Kadachi
		// Paolumu
		// Jyuratodus
		// Rathian
		// Legiana
		// Diablos
		// Rathalos

	// TODO: Create weapon & armor specified materials

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

	[Export(PropertyHint.Range, "1, 50, 0.1")]
	private float _offlineThresholdMult = 25f;

	public static Signals Signals = new Signals();
	public static PackedScenes PackedScenes;

	public static InterfaceType StartingInterface;
	public static float OfflineThresholdMult;

	public override void _EnterTree()
	{
		StartingInterface = _startingInterface;
		OfflineThresholdMult = _offlineThresholdMult;
	}

	public static Texture2D GetSpecialTypeIcon(SpecialType specialType)
	{
		string filePath = $"res://Assets/Images/Special/{specialType}Icon.png";
		return GetTexture(filePath);
	}

	public static Texture2D GetStatTypeIcon(StatType statType)
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
		string gradeColorString = GetGradeColorString(equipment.Grade);
		if (equipment is Weapon weapon)
		{
			if (weapon.Category == WeaponCategory.None) return null;

			filePath = $"res://Assets/Images/Weapons/{weapon.Category}Icon{gradeColorString}.png";
		}
		else if (equipment is Armor armor)
		{
			if (armor.Category == ArmorCategory.None) return null;
			
			filePath = $"res://Assets/Images/Armor/{armor.Category}Icon{gradeColorString}.png";
		}
		else if (equipment is PalicoWeapon palicoWeapon)
		{
			if (palicoWeapon.Type == PalicoEquipmentType.None) return null;

			filePath = $"res://Assets/Images/Palico/Palico{palicoWeapon.Type}{gradeColorString}.png";
		}
		else if (equipment is PalicoArmor palicoArmor)
		{
			if (palicoArmor.Type == PalicoEquipmentType.None) return null;

			if (palicoArmor.Type == PalicoEquipmentType.Head)
			{
				filePath = $"res://Assets/Images/Palico/Palico{palicoArmor.Type}{gradeColorString}.png";

			}
			else if (palicoArmor.Type == PalicoEquipmentType.Chest)
			{
				filePath = $"res://Assets/Images/Palico/Palico{palicoArmor.Type}{gradeColorString}.png";
			}
		}
		return GetTexture(filePath);
	}

	public static string GetGradeColorString(int grade) => grade switch
	{
		0 => "White",
		1 => "Green",
		2 => "Blue",
		3 => "Purple",
		4 => "Yellow",
		_ => "Red",
	};

	public static Texture2D GetTexture(string filePath)
	{
		return ResourceLoader.Load<Texture2D>(filePath);
	}

	public static string GetRomanNumeral(int grade) => grade switch
    {
        0 => "I",
        1 => "II",
        2 => "III",
        3 => "IV",
        4 => "V",
        5 => "VI",
        6 => "VII",
        7 => "VII",
        8 => "IX",
        9 => "X",
        _ => ""
    };

	public static int GetCraftingCost(int grade, int subGrade)
	{
		int[,] craftingCosts =
		{
			{ 10, 20, 30, 40, 50 },
			{ 300, 100, 150, 200, 250 },
			{ 600, 200, 300, 400, 500 },
			{ 900, 300, 450, 600, 750 },
			{ 1500, 500, 750, 1000, 1250 },
			{ 3000, 1000, 1500, 2000, 2500 },
			{ 6000, 2000, 3000, 4000, 5000 },
			{ 12000, 4000, 6000, 8000, 10000 },
			{ 30000, 10000, 15000, 20000, 25000 },
			{ 75000, 25000, 37500, 50000, 62500 }
		};
		return craftingCosts[grade, subGrade];
	}

	public static int GetMaterialWeight(int rarity) => rarity switch
	{
		0 => 7500,
		1 => 3000,
		2 => 1650,
		3 => 650,
		4 => 400,
		5 => 200,
		6 => 100,
		7 => 75,
		8 => 30,
		9 => 10,
		_ => throw new ArgumentOutOfRangeException("Rarity", $"Not expected rarity value: {rarity}")
	};

	public static Texture2D GetMaterialIcon(Material material)
	{
		string fileName = $"res://Assets/Images/Material/{material.Type}{material.Color}.png";
		return ResourceLoader.Load<Texture2D>(fileName);
	}

	public void GetRewards(int progressAmount, int zennyAmount)
	{
		Hunter.Zenny += zennyAmount;

		Hunter.Points += progressAmount;
		CheckHunterProgress();
	}

	private void AddHunterProgress(int progress)
	{
		Hunter.Points += progress;
	}

	private void CheckHunterProgress()
	{
		if (Hunter.Points < Hunter.PointsRequired) return;

		Hunter.Rank++;
		Hunter.Points -= Hunter.PointsRequired;
		GetMaxHunterProgress();
	}

	private void GetMaxHunterProgress()
	{
		if (Hunter.Rank < 100) Hunter.PointsRequired += 100;
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
		catch
		{
			string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Parse File - {fileName}";
			string result = "Returning Default/Null";
			PrintRich.PrintError(className, message, result);

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

		if (material == null)
		{
			string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
			string message = $"Couldn't Find Material - {materialName}";
			string result = "Returning Null";
			PrintRich.PrintError(className, message, result);
			
			return null;
		}

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
		List<MonsterMaterial> monsterMaterials = MonsterManager.GetMonsterMaterials(monster, monster.Level);
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

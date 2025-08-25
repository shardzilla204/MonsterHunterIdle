using System.Collections.Generic;
using Godot;
using GC = Godot.Collections;

namespace MonsterHunterIdle;

public enum TextColor
{
   White,
   Red,
   Orange,
   Yellow,
   Green,
   Purple,
   Blue,
   Pink,
   LightBlue,
   Brown,
}

public partial class PrintRich : Node
{
	[Export]
	private bool _isConsoleEnabled = true;

	[Export]
	private bool _areFileMessagesEnabled = false;

	[Export]
	private bool _areFilePathsVisible = false;

	public static bool IsConsoleEnabled;
	public static bool AreFileMessagesEnabled;
	public static bool AreFilePathsVisible;

	public override void _EnterTree()
	{
		IsConsoleEnabled = _isConsoleEnabled;
		AreFileMessagesEnabled = _areFileMessagesEnabled;
		AreFilePathsVisible = _areFilePathsVisible;
	}

	public static void Print(TextColor textColor, string text)
	{
		if (!IsConsoleEnabled) return;

		string coloredText = GetColoredMessage(textColor, text);
		GD.PrintRich(coloredText);
	}

	public static void PrintLine(TextColor textColor, string text)
	{
		if (!IsConsoleEnabled) return;

		Print(textColor, text);
		GD.Print(); // Spacing
	}

	public static void PrintSuccess(string fileName)
	{
		if (!IsConsoleEnabled) return;

		string loadSuccessMessage = $"{fileName} Successfully Loaded";
		PrintLine(TextColor.Green, loadSuccessMessage);
	}

	public static void PrintLocales(TextColor textColor)
	{
		if (!IsConsoleEnabled) return;

		List<LocaleMaterial> localeMaterials = MonsterHunterIdle.LocaleManager.Materials;

		List<LocaleType> localeTypes = new List<LocaleType>() { LocaleType.Forest, LocaleType.Desert, LocaleType.Swamp };
		foreach (LocaleType localeType in localeTypes)
		{
			List<LocaleMaterial> desiredMaterials = localeMaterials.FindAll(localeMaterial => localeMaterial.Locales.Contains(localeType));
			Print(textColor, $"{localeType} ({desiredMaterials.Count}):");
			foreach (LocaleMaterial localeMaterial in desiredMaterials)
			{
				Print(textColor, localeMaterial.Name);
			}
			GD.Print(); // Spacing
		}
	}

	public static void PrintMonsters(TextColor textColor)
	{
		if (!IsConsoleEnabled) return;

		List<Monster> monsters = MonsterHunterIdle.MonsterManager.Monsters;
		foreach (Monster monster in monsters)
		{
			PrintMonster(textColor, monster);
			GD.Print(); // Spacing
		}
	}

	private static void PrintMonster(TextColor textColor, Monster monster)
	{
		Print(textColor, $"Name: {monster.Name}");
		Print(textColor, $"Description: {monster.Description}");
		Print(textColor, $"Health: {monster.Health}");

		Print(textColor, "Specials:");
		PrintList(textColor, monster.Specials);

		Print(textColor, "Special Weaknesses:");
		PrintList(textColor, monster.SpecialWeaknesses);

		Print(textColor, "Locales:");
		PrintList(textColor, monster.Locales);
	}

	private static void PrintList<T>(TextColor textColor, List<T> values)
	{
		foreach (T value in values)
		{
			Print(textColor, $"\t{value}");
		}

		if (values.Count == 0) Print(textColor, $"\tNone");
	}

	public static string GetMonsterMessage(Monster monster)
	{
		string monsterMessage = $"★{monster.Level} {monster.Name}";
		string coloredMonsterMessage = GetColoredMessage(TextColor.Yellow, monsterMessage);
		return coloredMonsterMessage;
	}

	private static string GetColoredMessage(TextColor textColor, string text)
	{
		string textColorString = GetColorHex(textColor);
		return $"[color={textColorString}]{text}[/color]";
	}

	public static void PrintEquipmentInfo(TextColor textColor, Equipment equipment)
	{
		if (equipment is Weapon weapon)
		{
			Print(textColor, "Weapon: ");
			Print(textColor, $"\tName: {weapon.Name}");
			Print(textColor, $"\tCategory: {weapon.Category}");
			Print(textColor, $"\tTree: {weapon.Tree}");
			Print(textColor, $"\tGrade: {weapon.Grade}");
			Print(textColor, $"\tSub Grade: {weapon.SubGrade}");
		}
		else if (equipment is Armor armor)
		{
			Print(textColor, "Armor: ");
			Print(textColor, $"\tName: {armor.Name}");
			Print(textColor, $"\tCategory: {armor.Category}");
			Print(textColor, $"\tSet: {armor.Set}");
			Print(textColor, $"\tGrade: {armor.Grade}");
			Print(textColor, $"\tSub Grade: {armor.SubGrade}");
		}
		GD.Print(); // Spacing
	}

	public static void PrintMaterials()
	{
		if (!IsConsoleEnabled) return;

		GD.Print("Item Box: ");
		string textColorString = TextColor.Orange.ToString().ToUpper();
		for (int i = 0; i < MonsterHunterIdle.ItemBox.Materials.Count; i++)
		{
			GD.PrintRich($"\t[color={textColorString}]{MonsterHunterIdle.ItemBox.Materials[i].Name}[/color]");
		}
		GD.Print();
	}

	public static void PrintEncounterRewards(int points, int zenny, List<MonsterMaterial> materialRewards)
	{
		string pointsGainedMessage = $"\t+ {points} Hunter Points";
		Print(TextColor.Orange, pointsGainedMessage);

		string zennyGainedMessage = $"\t+ {zenny} Zenny";
		Print(TextColor.Orange, zennyGainedMessage);

		foreach (MonsterMaterial materialReward in materialRewards)
		{
			string materialGainedMessage = $"\t+ {materialReward.Name}";
			Print(TextColor.Orange, materialGainedMessage);
		}

		GD.Print(); // Spacing
	}

	public static string GetColorHex(TextColor textColor) => textColor switch
	{
		TextColor.Red => "FF4040",
		TextColor.Orange => "F88158",
		TextColor.Yellow => "E9D66B",
		TextColor.Green => "76CD26",
		TextColor.Purple => "CA9BF7",
		TextColor.Blue => "6495ED",
		TextColor.Pink => "FF69B4",
		TextColor.LightBlue => "ADD8E6",
		TextColor.Brown => "C4A484",
		_ => "FFFFFF",
	};

	public static void PrintTimeDifference(GC.Dictionary<string, int> timeDifference)
	{
		string timeDifferenceMessage = $"Time Difference: {GetTimeString(timeDifference)}";
		PrintLine(TextColor.Purple, timeDifferenceMessage);
	}

	public static string GetTimeString(GC.Dictionary<string, int> time)
	{
		return $"\n\tHours: {time["hour"]}\n\tMinutes: {time["minute"]}\n\tSeconds: {time["second"]}";
	}
}

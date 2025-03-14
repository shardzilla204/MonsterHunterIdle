using Godot;

namespace MonsterHunterIdle;

public enum TextColor
{
	Red,
	Orange,
	Yellow,
	Green,
	Blue,
	Purple
}

public partial class PrintRich : Node
{
	public static void Print(TextColor textColor, string text)
   {
      if (!MonsterHunterIdle.AreConsoleMessagesEnabled) return;

      string textColorString = textColor.ToString().ToUpper();
      GD.PrintRich($"[color={textColorString}]{text}[/color]");
      GD.Print();
   }

	public static string GetMonsterMessage(Monster monster)
	{
		string monsterLevel = $"★{monster.Level}";
		return $"★{monsterLevel} {monster.Name}";
	}

	public static void PrintMaterials()
	{
		GD.Print("Item Box: ");
		string textColorString = TextColor.Orange.ToString().ToUpper();
		for (int i = 0; i < MonsterHunterIdle.ItemBox.Materials.Count; i++)
		{
			GD.PrintRich($"\t[color={textColorString}]{MonsterHunterIdle.ItemBox.Materials[i].Name}[/color]");
		}
		GD.Print();
	}
}

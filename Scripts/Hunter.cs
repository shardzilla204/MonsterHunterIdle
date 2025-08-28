using Godot;

namespace MonsterHunterIdle;

public partial class Hunter : Node
{
	public Hunter(int zenny)
	{
		Zenny = zenny;
	}

	public static int Rank = 1;
	public const int MaxRank = 999;

	public static int Points = 0;
	public static int PointsRequired = 100;

	public static int Zenny = 0;

	public static Weapon Weapon = new Weapon();

	public static Armor Head = new Armor(ArmorCategory.Head);
	public static Armor Chest = new Armor(ArmorCategory.Chest);
	public static Armor Arm = new Armor(ArmorCategory.Arm);
	public static Armor Waist = new Armor(ArmorCategory.Waist);
	public static Armor Leg = new Armor(ArmorCategory.Leg);

	public static void ResetData()
	{
		Zenny = HunterManager.StartingZenny;

		Rank = 1;

		Points = 0;
		PointsRequired = 100;

		Weapon = new Weapon();

		Head = new Armor(ArmorCategory.Head);
		Chest = new Armor(ArmorCategory.Chest);
		Arm = new Armor(ArmorCategory.Arm);
		Waist = new Armor(ArmorCategory.Waist);
		Leg = new Armor(ArmorCategory.Leg);
	}
}
using System.Collections.Generic;
using Godot;

namespace MonsterHunterIdle;

public partial class Hunter : Node
{
	public Hunter(int zenny)
	{
		Zenny = zenny;
	}

	public int Rank = 1;
	public const int MaxRank = 999;

	public int Points = 0;
	public int PointsRequired = 100;

	public int Zenny = 0;

	public List<Weapon> Weapons = new List<Weapon>();
	public Weapon Weapon = new Weapon();

	public List<Armor> Armor = new List<Armor>();
	public Armor Head = new Armor(ArmorCategory.Head);
	public Armor Chest = new Armor(ArmorCategory.Chest);
	public Armor Arm = new Armor(ArmorCategory.Arm);
	public Armor Waist = new Armor(ArmorCategory.Waist);
	public Armor Leg = new Armor(ArmorCategory.Leg);
}
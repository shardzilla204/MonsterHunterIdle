using Godot;

namespace MonsterHunterIdle;

public partial class PalicoData : Resource
{
	[Export]
	public string Name = "Palico";

	[Export]
	public PalicoMode Mode = PalicoMode.Gather;

	[Export]
	public int Attack = 25;

	[Export]
	public int Defense = 0;

	[Export]
	public int Affinity = 0;

	[Export]
	public PalicoWeaponData Weapon;

	[Export]
	public PalicoArmorData Helmet;

	[Export]
	public PalicoArmorData Armor;
}

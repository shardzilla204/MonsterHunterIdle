using Godot;

namespace MonsterHunterIdle;

public partial class PalicoDataHolder : Node
{
	[Export]
	public PalicoWeaponDataHolder Weapon;

	[Export]
	public PalicoArmorDataHolder Armor;
}

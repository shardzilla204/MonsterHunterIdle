using Godot;

namespace MonsterHunterIdle;

public partial class PlayerDataHolder : Node
{
	[Export]
	public PlayerWeaponDataHolder Weapon;

	[Export]
	public PlayerArmorDataHolder Armor;
}

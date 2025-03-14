using Godot;

namespace MonsterHunterIdle;

public enum PalicoMode
{
	Gather,
	Hunt
}

public partial class Palico : Node
{
   public new string Name;
   public PalicoMode Mode;

   public int Attack;
   public int Affinity;
   public int Defense;

   public PalicoWeapon Weapon;
   public PalicoArmor Helmet;
   public PalicoArmor Chest;
}
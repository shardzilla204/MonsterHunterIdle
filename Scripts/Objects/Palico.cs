using Godot;

namespace MonsterHunterIdle;

public enum PalicoMode
{
	Gather,
	Hunt
}

public partial class Palico : Node
{
   public new string Name = "Palico";

   public int Attack = 5;
   public int Affinity = 0;
   public int Defense = 10;

   public PalicoWeapon Weapon = new PalicoWeapon();
   public PalicoArmor Head = new PalicoArmor(PalicoEquipmentType.Head);
   public PalicoArmor Chest = new PalicoArmor(PalicoEquipmentType.Chest);
}
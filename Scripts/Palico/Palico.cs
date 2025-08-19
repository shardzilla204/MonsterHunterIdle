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

   public Weapon Weapon;
   public Armor Head;
   public Armor Chest;
}
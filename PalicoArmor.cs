using Godot;
using System.Collections.Generic;

namespace MonsterHunterIdle;

public enum PalicoArmorType 
{
   Helmet,
   Chest
}

public partial class PalicoArmor : Node
{
	public int Defense;
	public List<ElementalResistance> Resistances;
	public int Health;
   public PalicoArmorType ArmorType;
}
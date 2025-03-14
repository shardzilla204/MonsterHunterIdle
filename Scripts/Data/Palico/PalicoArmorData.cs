using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

[GlobalClass]
public partial class PalicoArmorData : PalicoEquipmentData
{
	[Export]
	private int _defense; 

	[Export]
	private Array<ElementalResistance> _resistances = new Array<ElementalResistance>();

	[Export]
	private int _health;

	public int Defense => _defense;
	public List<ElementalResistance> Resistances => _resistances.ToList();
	public int Health => _health;
}

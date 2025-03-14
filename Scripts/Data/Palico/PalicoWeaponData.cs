using Godot;

namespace MonsterHunterIdle;

[GlobalClass]
public partial class PalicoWeaponData : PalicoEquipmentData
{
	[Export]
	private int _attack;

	[Export]
	private int _affinity;

	[Export]
	private int _defense; 

	public int Attack => _attack;
	public int Affinity => _affinity;
	public int Defense => _defense;
}

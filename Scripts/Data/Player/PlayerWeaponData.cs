using Godot;

namespace MonsterHunterIdle;

public enum PlayerWeaponType
{
    SwordShield
}

[GlobalClass]
public partial class PlayerWeaponData : PlayerEquipmentData
{
    [Export]
    private PlayerWeaponType _weaponType;

	[Export]
	private int _attack;

	[Export]
	private int _affinity;

	[Export]
	private int _defense; 

    public PlayerWeaponType WeaponType => _weaponType;
	public int Attack => _attack;
	public int Affinity => _affinity;
	public int Defense => _defense;
}

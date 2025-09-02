using System;
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class PalicoWeapon : PalicoEquipment
{
    public PalicoWeapon(){}

    public PalicoWeapon(WeaponTree tree)
    {
        Tree = tree;
    }

    public new PalicoEquipmentType Type = PalicoEquipmentType.Weapon;
    public int Attack = 0;
    public int SpecialAttack = 0;
    public SpecialType Special;
    public int Affinity = 0;
    public WeaponTree Tree = WeaponTree.None;

    public override void SetEquipment(Dictionary<string, Variant> dictionary)
    {
        Name = dictionary["Name"].As<string>();

        string specialTypeString = dictionary["Special"].As<string>();
        Special = Enum.Parse<SpecialType>(specialTypeString);

        Attack = PalicoEquipmentManager.GetAttackValue(this);
        SpecialAttack = PalicoEquipmentManager.GetSpecialValue(this);
    }
}
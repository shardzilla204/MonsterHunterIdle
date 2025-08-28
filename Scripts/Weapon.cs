using System;
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public enum WeaponCategory
{
    None = -1,
    SwordAndShield,
    GreatSword,
    LongSword
}

public enum WeaponTree
{
    None = -1,
    Ore,
    GreatJagras,
    KuluYaKu,
    PukeiPukei,
    Barroth,
    GreatGirros,
    TobiKadachi,
    Paolumu,
    Jyuratodus,
    Anjanath,
    Rathian,
    Legiana,
    Diablos,
    Rathalos
}

public partial class Weapon : Equipment
{
    public Weapon() { }

    public Weapon(WeaponCategory category, WeaponTree tree)
    {
        Category = category;
        Tree = tree;
    }

    public int Attack = 0;
    public int SpecialAttack = 0;
    public SpecialType Special;
    public int Affinity = 0;
    public WeaponCategory Category = WeaponCategory.None;
    public WeaponTree Tree = WeaponTree.None;

    public override void SetEquipment(Dictionary<string, Variant> dictionary)
    {
        Array<string> names = dictionary["Names"].As<Array<string>>();
        EquipmentManager.SetWeaponName(this, names);

        string specialTypeString = dictionary["Special"].As<string>();
        Special = Enum.Parse<SpecialType>(specialTypeString);

        Attack = EquipmentManager.GetAttackValue(this);
        SpecialAttack = EquipmentManager.GetSpecialValue(this);
    }
}
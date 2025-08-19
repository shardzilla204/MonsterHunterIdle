using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public enum EquipmentType
{
    None = -1,
    Weapon,
    Armor,
}

public enum WeaponCategory
{
    None = -1,
    SwordAndShield
}

public enum WeaponTree
{
    None = -1,
    Ore,
    GreatJagras
}

public enum ArmorCategory
{
    None = -1,
    Head,
    Chest,
    Arm,
    Waist,
    Leg
}

public enum ArmorSet
{
    None = -1,
    Leather,
    Jagras
}

public enum PalicoEquipmentCategory
{
    None = -1,
    Weapon,
    Head,
    Chest
}

public abstract partial class Equipment : Node
{
    public new string Name = "";
    public int Grade = 0;
    public int SubGrade = 0;

    public abstract void SetEquipment(Dictionary<string, Variant> dictionary);
}
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public enum WeaponCategory
{
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

public enum PalicoCategory
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
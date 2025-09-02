using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public enum EquipmentType
{
    None = -1,
    Weapon,
    Armor,
}

public abstract partial class Equipment : Node
{
    public new string Name = "";
    public int Grade = 0;
    public int SubGrade = 0;
    public EquipmentType Type = EquipmentType.None;

    public abstract void SetEquipment(Dictionary<string, Variant> dictionary);
}
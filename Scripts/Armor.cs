using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class Armor : Equipment
{
    public Armor() { }

    public Armor(ArmorCategory category)
    {
        Category = category;
    }

    public Armor(ArmorCategory category, ArmorSet set)
    {
        Category = category;
        Set = set;
    }

    public int Defense = 0;
    public ArmorCategory Category = ArmorCategory.None;
    public new ArmorSet Set = ArmorSet.None;

    public override void SetEquipment(Dictionary<string, Variant> dictionary)
    {
        Name = dictionary["Name"].As<string>();
        Defense = MonsterHunterIdle.EquipmentManager.GetDefenseValue(Grade, SubGrade);
    }
}
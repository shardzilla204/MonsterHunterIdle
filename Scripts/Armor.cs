using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class Armor : Equipment
{
    public Armor() {}

    public Armor(ArmorCategory category)
    {
        Category = category;
        Set = ArmorSet.None;
    }

    public Armor(ArmorCategory category, ArmorSet set)
    {
        Category = category;
        Set = set;
    }

    public int Defense = 0;
    public ArmorCategory Category;
    public new ArmorSet Set;

    public override void SetEquipment(Dictionary<string, Variant> dictionary)
    {
        Name = dictionary["Name"].As<string>();

        Array<int> defenseValues = dictionary["Defense"].As<Array<int>>();
        Defense = defenseValues[SubGrade];
    }
}
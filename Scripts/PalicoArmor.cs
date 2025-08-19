using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class PalicoArmor : Equipment
{
    public PalicoArmor(){}

    public PalicoArmor(PalicoEquipmentCategory category)
    {
        Category = category;
    }

    public PalicoArmor(PalicoEquipmentCategory category, ArmorSet set)
    {
        Category = category;
        Set = set;
    }

    public int Defense = 0;
    public PalicoEquipmentCategory Category = PalicoEquipmentCategory.None;
    public new ArmorSet Set = ArmorSet.None;

    public override void SetEquipment(Dictionary<string, Variant> dictionary)
    {
        Name = dictionary["Name"].As<string>();

        Array<int> defenseValues = dictionary["Defense"].As<Array<int>>();
        Defense = defenseValues[SubGrade];
    }
}
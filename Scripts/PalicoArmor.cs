using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class PalicoArmor : Armor
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

    public new PalicoEquipmentCategory Category = PalicoEquipmentCategory.None;

    public override void SetEquipment(Dictionary<string, Variant> dictionary)
    {
        Name = dictionary["Name"].As<string>();

        Array<int> defenseValues = dictionary["Defense"].As<Array<int>>();
        Defense = defenseValues[SubGrade];
    }
}
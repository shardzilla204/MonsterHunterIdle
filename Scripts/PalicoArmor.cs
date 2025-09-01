using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class PalicoArmor : PalicoEquipment
{
    public PalicoArmor(){}
    
    public PalicoArmor(PalicoEquipmentType type)
    {
        Type = type;
    }

    public PalicoArmor(PalicoEquipmentType type, ArmorSet set)
    {
        Type = type;
        Set = set;
    }

    public int Defense = 0;
    public new ArmorSet Set = ArmorSet.None;

    public override void SetEquipment(Dictionary<string, Variant> dictionary)
    {
        Array<string> armorNames = dictionary[Set.ToString()].As<Array<string>>();
        string armorName = armorNames[(int) Type - 1]; // Go down an index for head & chest
        Name = PalicoEquipmentManager.GetArmorName(this, armorName);

        Defense = EquipmentManager.GetDefenseValue(Grade, SubGrade) / 2;
    }
}
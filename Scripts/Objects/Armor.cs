using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

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
    Jagras,
    Kulu,
    Pukei,
    Barroth,
    Girros,
    Kadachi,
    Paolumu,
    Jyuratodus,
    Anjanath,
    Rathian,
    Legiana,
    Diablos,
    Rathalos
}

public partial class Armor : Equipment
{
    public Armor(){}

    public Armor(ArmorCategory category)
    {
        Category = category;
    }

    public Armor(ArmorCategory category, ArmorSet set)
    {
        Category = category;
        Set = set;
    }

    public new EquipmentType Type = EquipmentType.Armor;
    public int Defense = 0;
    public ArmorCategory Category = ArmorCategory.None;
    public new ArmorSet Set = ArmorSet.None;

    public override void SetEquipment(Dictionary<string, Variant> dictionary)
    {
        string setName = Set.ToString();
        Array<string> names = dictionary[setName].As<Array<string>>();
        EquipmentManager.SetArmorName(this, names);

        Defense = EquipmentManager.GetArmorDefense(Grade, SubGrade);
    }
}
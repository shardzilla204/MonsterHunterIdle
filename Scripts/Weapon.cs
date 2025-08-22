using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class Weapon : Equipment
{
    public Weapon(){}

    public Weapon(WeaponCategory category, WeaponTree tree)
    {
        Category = category;
        Tree = tree;
    }

    public int Attack = 0;
    public int Special = 0;
    public float Affinity = 0;
    public WeaponCategory Category = WeaponCategory.None;
    public WeaponTree Tree = WeaponTree.None;

    public override void SetEquipment(Dictionary<string, Variant> dictionary)
    {
        Name = dictionary["Name"].As<string>();

        Attack = MonsterHunterIdle.EquipmentManager.GetAttackValue(this);
        Special = MonsterHunterIdle.EquipmentManager.GetSpecialValue(this);
    }
}
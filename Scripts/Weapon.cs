using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class Weapon : Equipment
{
    public Weapon()
    {
        Tree = WeaponTree.None;
    }

    public Weapon(WeaponCategory category, WeaponTree tree)
    {
        Category = category;
        Tree = tree;
    }

    public int Attack = 0;
    public float Affinity = 0;
    public WeaponCategory Category;
    public WeaponTree Tree;

    public override void SetEquipment(Dictionary<string, Variant> dictionary)
    {
        Name = dictionary["Name"].As<string>();

        Array<int> attackValues = dictionary["Attack"].As<Array<int>>();
        Attack = attackValues[SubGrade];

        Array<int> affinityValues = dictionary["Affinity"].As<Array<int>>();
        Affinity = affinityValues.Count != 0 ? affinityValues[SubGrade] : 0;
    }
}
using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class PalicoWeapon : Equipment
{
    public PalicoWeapon(){}

    public PalicoWeapon(WeaponTree tree)
    {
        Tree = tree;
    }

    public int Attack = 0;
    public float Affinity = 0;
    public PalicoEquipmentCategory Category = PalicoEquipmentCategory.Weapon;
    public WeaponTree Tree = WeaponTree.None;

    public override void SetEquipment(Dictionary<string, Variant> dictionary)
    {
        Name = dictionary["Name"].As<string>();

        Array<int> attackValues = dictionary["Attack"].As<Array<int>>();
        Attack = attackValues[SubGrade];

        Array<int> affinityValues = dictionary["Affinity"].As<Array<int>>();
        Affinity = affinityValues.Count != 0 ? affinityValues[SubGrade] : 0;
    }
}
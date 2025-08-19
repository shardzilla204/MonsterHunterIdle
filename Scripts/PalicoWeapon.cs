using Godot;
using Godot.Collections;

namespace MonsterHunterIdle;

public partial class PalicoWeapon : Weapon
{
    public PalicoWeapon(){}

    public PalicoWeapon(WeaponTree tree)
    {
        Tree = tree;
    }

    public new PalicoEquipmentCategory Category = PalicoEquipmentCategory.Weapon;

    public override void SetEquipment(Dictionary<string, Variant> dictionary)
    {
        Name = dictionary["Name"].As<string>();

        Array<int> attackValues = dictionary["Attack"].As<Array<int>>();
        Attack = attackValues[SubGrade];

        Array<int> affinityValues = dictionary["Affinity"].As<Array<int>>();
        Affinity = affinityValues.Count != 0 ? affinityValues[SubGrade] : 0;
    }
}
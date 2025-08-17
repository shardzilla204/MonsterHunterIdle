using Godot;

namespace MonsterHunterIdle;

public partial class LoadoutInfo : NinePatchRect
{
    [Export]
    private Container _equipmentInfoContainer;

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.ChangeEquipmentButtonPressed -= ChangeEquipmentButtonPressed;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.ChangeEquipmentButtonPressed += ChangeEquipmentButtonPressed;
    }

    public override void _Ready()
    {
        AddLoadoutInfo();
    }

    private void AddLoadoutInfo()
    {
        Hunter hunter = MonsterHunterIdle.HunterManager.Hunter;

        Weapon hunterWeapon = hunter.Weapon;
        EquipmentInfo weaponInfoNode = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(hunterWeapon);
        _equipmentInfoContainer.AddChild(weaponInfoNode);

        Armor hunterHeadArmor = hunter.Head;
        EquipmentInfo headArmorInfoNode = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(hunterHeadArmor);
        _equipmentInfoContainer.AddChild(headArmorInfoNode);

        Armor hunterChestArmor = hunter.Chest;
        EquipmentInfo chestArmorInfoNode = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(hunterChestArmor);
        _equipmentInfoContainer.AddChild(chestArmorInfoNode);

        Armor hunterArmArmor = hunter.Arm;
        EquipmentInfo armArmorInfoNode = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(hunterArmArmor);
        _equipmentInfoContainer.AddChild(armArmorInfoNode);

        Armor hunterWaistArmor = hunter.Waist;
        EquipmentInfo waistArmorInfoNode = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(hunterWaistArmor);
        _equipmentInfoContainer.AddChild(waistArmorInfoNode);

        Armor hunterLegArmor = hunter.Leg;
        EquipmentInfo legArmorInfoNode = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(hunterLegArmor);
        _equipmentInfoContainer.AddChild(legArmorInfoNode);
    }

    private void ChangeEquipmentButtonPressed(Equipment equipment)
    {
        EquipmentSelectionInterface equipmentSelectionInterface = MonsterHunterIdle.PackedScenes.GetEquipmentSelectionInterface(equipment);
        if (equipmentSelectionInterface == null) return;

        Container loadoutInterface = GetParentOrNull<Container>();
        loadoutInterface.AddSibling(equipmentSelectionInterface);
    }
}

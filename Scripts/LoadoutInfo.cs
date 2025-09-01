using Godot;

namespace MonsterHunterIdle;

public partial class LoadoutInfo : NinePatchRect
{
    [Export]
    private Container _equipmentInfoContainer;

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.ChangeEquipmentButtonPressed -= OnChangeEquipmentButtonPressed;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.ChangeEquipmentButtonPressed += OnChangeEquipmentButtonPressed;
    }

    public override void _Ready()
    {
        AddLoadoutInfo();
    }

    // * START - Signal Methods
    private void OnChangeEquipmentButtonPressed(Equipment equipment)
    {
        EquipmentSelectionInterface equipmentSelectionInterface = MonsterHunterIdle.PackedScenes.GetEquipmentSelectionInterface(equipment);
        if (equipmentSelectionInterface == null) return;

        Container loadoutInterface = GetParentOrNull<Container>();
        loadoutInterface.AddSibling(equipmentSelectionInterface);
    }
    // * END - Signal Methods

    private void AddLoadoutInfo()
    {
        EquipmentInfo weaponInfoNode = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(Hunter.Weapon);
        _equipmentInfoContainer.AddChild(weaponInfoNode);

        EquipmentInfo headArmorInfoNode = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(Hunter.Head);
        _equipmentInfoContainer.AddChild(headArmorInfoNode);

        EquipmentInfo chestArmorInfoNode = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(Hunter.Chest);
        _equipmentInfoContainer.AddChild(chestArmorInfoNode);

        EquipmentInfo armArmorInfoNode = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(Hunter.Arm);
        _equipmentInfoContainer.AddChild(armArmorInfoNode);

        EquipmentInfo waistArmorInfoNode = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(Hunter.Waist);
        _equipmentInfoContainer.AddChild(waistArmorInfoNode);

        EquipmentInfo legArmorInfoNode = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(Hunter.Leg);
        _equipmentInfoContainer.AddChild(legArmorInfoNode);
    }
}

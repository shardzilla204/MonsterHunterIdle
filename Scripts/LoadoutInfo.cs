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
        EquipmentInfo weaponInfo = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(Hunter.Weapon);
        _equipmentInfoContainer.AddChild(weaponInfo);

        EquipmentInfo headInfo = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(Hunter.Head);
        _equipmentInfoContainer.AddChild(headInfo);

        EquipmentInfo chestInfo = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(Hunter.Chest);
        _equipmentInfoContainer.AddChild(chestInfo);

        EquipmentInfo armInfo = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(Hunter.Arm);
        _equipmentInfoContainer.AddChild(armInfo);

        EquipmentInfo waistInfo = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(Hunter.Waist);
        _equipmentInfoContainer.AddChild(waistInfo);

        EquipmentInfo legInfo = MonsterHunterIdle.PackedScenes.GetEquipmentInfo(Hunter.Leg);
        _equipmentInfoContainer.AddChild(legInfo);
    }
}

using Godot;
using System.Collections.Generic;

namespace MonsterHunterIdle;

public partial class EquipmentSelectionInterface : NinePatchRect
{
    [Export]
    private Container _equipmentOptionContainer;

    private List<EquipmentOptionButton> _equipmentOptionButtons = new List<EquipmentOptionButton>();
    private EquipmentOptionInfoInterface _equipmentOptionInfoInterface;

    private Equipment _equipment;

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.InterfaceChanged -= OnInterfaceChanged;
        MonsterHunterIdle.Signals.ChangeEquipmentButtonPressed -= OnChangeEquipmentButtonPressed;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
        MonsterHunterIdle.Signals.ChangeEquipmentButtonPressed += OnChangeEquipmentButtonPressed;
    }

    public override void _Ready()
    {
        // No reason to display if there are equipment count is 0
        if (_equipmentOptionButtons.Count == 0)
        {
            QueueFree();
            return;
        }

        // Toggle the equipment button that is currently being used
        EquipmentOptionButton equipmentOptionButton = _equipmentOptionButtons.Find(option => option.Equipment.Name == _equipment.Name);
        equipmentOptionButton.OnToggled(true);
    }

    // Add equipment buttons that correspond to the equipment type
    public void SetEquipment(Equipment equipment)
    {
        _equipment = equipment;
        if (equipment is Weapon)
        {
            foreach (Weapon weapon in MonsterHunterIdle.HunterManager.Hunter.Weapons)
            {
                AddEquipmentOption(weapon);
            }
        }
        else if (equipment is Armor targetArmor)
        {
            List<Armor> armorList = MonsterHunterIdle.HunterManager.Hunter.Armor.FindAll(armor => armor.Category == targetArmor.Category);
            foreach (Armor armor in armorList)
            {
                AddEquipmentOption(armor);
            }
        }
    }

    private void AddEquipmentOption(Equipment equipment)
    {
        EquipmentOptionButton equipmentOptionButton = MonsterHunterIdle.PackedScenes.GetEquipmentOptionButton(equipment);
        _equipmentOptionButtons.Add(equipmentOptionButton);
        _equipmentOptionContainer.AddChild(equipmentOptionButton);
        equipmentOptionButton.Pressed += () =>
        {
            ShowEquipmentPopup(equipmentOptionButton.Equipment);

            // Unpress the other buttons   
            List<EquipmentOptionButton> otherButtons = _equipmentOptionButtons.FindAll(button => button != equipmentOptionButton);
            foreach (EquipmentOptionButton otherButton in otherButtons)
            {
                otherButton.OnToggled(false);
            }
        };
    }

    private void ShowEquipmentInfo(Equipment equipment)
    {
        int index = IsInstanceValid(_equipmentOptionInfoInterface) ? 2 : 1;
        if (IsInstanceValid(_equipmentOptionInfoInterface)) _equipmentOptionInfoInterface.QueueFree();

        _equipmentOptionInfoInterface = MonsterHunterIdle.PackedScenes.GetEquipmentOptionInfoInterface(equipment);
        Container interfaceContainer = GetParentOrNull<Container>();
        AddSibling(_equipmentOptionInfoInterface);
        interfaceContainer.MoveChild(_equipmentOptionInfoInterface, index); // Move the node behind the monster interface

        // GD.Print($"Showing Equipment Info For: {equipment.Name}");
    }

    private void ShowEquipmentPopup(Equipment equipment)
    {
        EquipmentOptionInfoInterface equipmentOptionInfoInterface = MonsterHunterIdle.PackedScenes.GetEquipmentOptionInfoInterface(equipment);
        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.Popup, equipmentOptionInfoInterface);
    }

    private void OnInterfaceChanged(InterfaceType interfaceType)
    {
        QueueFree();
    }

    private void OnChangeEquipmentButtonPressed(Equipment equipment)
    {
        QueueFree();
    }
}

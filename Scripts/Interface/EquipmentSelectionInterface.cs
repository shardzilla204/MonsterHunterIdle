using Godot;
using System.Collections.Generic;

namespace MonsterHunterIdle;

public partial class EquipmentSelectionInterface : NinePatchRect
{
    [Export]
    private Container _equipmentInfoButtonContainer;

    private List<EquipmentInfoButton> _equipmentInfoButtons = new List<EquipmentInfoButton>();
    private EquipmentInfoPopup _equipmentInfoPopup;

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
        if (_equipmentInfoButtons.Count == 0)
        {
            QueueFree();
            return;
        }

        // Toggle the equipment button that is currently being used
        EquipmentInfoButton equipmentInfoButton = _equipmentInfoButtons.Find(option => option.Equipment.Name == _equipment.Name);
        equipmentInfoButton.OnToggled(true);
    }

    // Add equipment buttons that correspond to the equipment type
    public void SetEquipment(Equipment equipment)
    {
        _equipment = equipment;
        if (equipment is Weapon)
        {
            foreach (Weapon weapon in MonsterHunterIdle.EquipmentManager.CraftedWeapons)
            {
                AddEquipmentOption(weapon);
            }
        }
        else if (equipment is Armor targetArmor)
        {
            List<Armor> armorList = MonsterHunterIdle.EquipmentManager.CraftedArmor.FindAll(armor => armor.Category == targetArmor.Category);
            foreach (Armor armor in armorList)
            {
                AddEquipmentOption(armor);
            }
        }
    }

    private void AddEquipmentOption(Equipment equipment)
    {
        EquipmentInfoButton equipmentInfoButton = MonsterHunterIdle.PackedScenes.GetEquipmentInfoButton(equipment);
        _equipmentInfoButtons.Add(equipmentInfoButton);
        _equipmentInfoButtonContainer.AddChild(equipmentInfoButton);
        equipmentInfoButton.Pressed += () =>
        {
            ShowEquipmentPopup(equipmentInfoButton.Equipment);

            // Unpress the other buttons   
            List<EquipmentInfoButton> otherButtons = _equipmentInfoButtons.FindAll(button => button != equipmentInfoButton);
            foreach (EquipmentInfoButton otherButton in otherButtons)
            {
                otherButton.OnToggled(false);
            }
        };
    }

    private void ShowEquipmentInfo(Equipment equipment)
    {
        int index = IsInstanceValid(_equipmentInfoPopup) ? 2 : 1;
        if (IsInstanceValid(_equipmentInfoPopup)) _equipmentInfoPopup.QueueFree();

        _equipmentInfoPopup = MonsterHunterIdle.PackedScenes.GetEquipmentInfoPopup(equipment);
        Container interfaceContainer = GetParentOrNull<Container>();
        AddSibling(_equipmentInfoPopup);
        interfaceContainer.MoveChild(_equipmentInfoPopup, index); // Move the node behind the monster interface

        // GD.Print($"Showing Equipment Info For: {equipment.Name}");
    }

    private void ShowEquipmentPopup(Equipment equipment)
    {
        EquipmentInfoPopup equipmentInfoPopup = MonsterHunterIdle.PackedScenes.GetEquipmentInfoPopup(equipment);
        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.Popup, equipmentInfoPopup);
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

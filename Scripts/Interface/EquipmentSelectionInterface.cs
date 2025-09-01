using Godot;
using System.Collections.Generic;

namespace MonsterHunterIdle;

public partial class EquipmentSelectionInterface : NinePatchRect
{
    [Export]
    private Container _equipmentSelectionButtonContainer;

    private List<EquipmentSelectionButton> _equipmentSelectionButtons = new List<EquipmentSelectionButton>();
    private EquipmentInfoPopup _equipmentInfoPopup;

    private Equipment _equipment;

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.InterfaceChanged -= OnInterfaceChanged;
        MonsterHunterIdle.Signals.ChangeEquipmentButtonPressed -= OnChangeEquipmentButtonPressed;
        MonsterHunterIdle.Signals.ChangePalicoEquipmentButtonPressed -= OnChangePalicoEquipmentButtonPressed;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
        MonsterHunterIdle.Signals.ChangeEquipmentButtonPressed += OnChangeEquipmentButtonPressed;
        MonsterHunterIdle.Signals.ChangePalicoEquipmentButtonPressed += OnChangePalicoEquipmentButtonPressed;
    }

    public override void _Ready()
    {
        // No reason to display if there are equipment count is 0
        if (_equipmentSelectionButtons.Count == 0)
        {
            QueueFree();
            return;
        }

        // Toggle the equipment button that is currently being used
        if (_equipment == null) return;

        EquipmentSelectionButton equipmentSelectionButton = _equipmentSelectionButtons.Find(option => option.Equipment.Name == _equipment.Name);
        equipmentSelectionButton.OnToggled(true);
    }

    // * START - Signal Methods
    private void OnInterfaceChanged(InterfaceType interfaceType)
    {
        QueueFree();
    }

    private void OnChangeEquipmentButtonPressed(Equipment equipment)
    {
        QueueFree();
    }
    private void OnChangePalicoEquipmentButtonPressed(PalicoEquipmentType equipmentType)
    {
        QueueFree();
    }
    // * END - Signal Methods

    // Add equipment buttons that correspond to the equipment type
    public void SetEquipment(Equipment equipment)
    {
        _equipment = equipment;
        if (equipment is Weapon)
        {
            foreach (Weapon weapon in EquipmentManager.CraftedWeapons)
            {
                AddEquipmentOption(weapon);
            }
        }
        else if (equipment is Armor targetArmor)
        {
            List<Armor> armorPieces = EquipmentManager.CraftedArmor.FindAll(armor => armor.Category == targetArmor.Category);
            foreach (Armor armorPiece in armorPieces)
            {
                AddEquipmentOption(armorPiece);
            }
        }
        else if (equipment is PalicoWeapon targetPalicoWeapon)
        {
            List<PalicoWeapon> weapons = PalicoEquipmentManager.FindCraftedWeapons(targetPalicoWeapon);
            foreach (PalicoWeapon weapon in weapons)
            {
                AddEquipmentOption(weapon);
            }
        }
        else if (equipment is PalicoArmor targetPalicoArmor)
        {
            List<PalicoArmor> armorPieces = PalicoEquipmentManager.FindCraftedArmor(targetPalicoArmor);
            foreach (PalicoArmor armorPiece in armorPieces)
            {
                AddEquipmentOption(armorPiece);
            }
        }
    }

    public void SetEquipment(PalicoEquipmentType equipmentType)
    {
        if (equipmentType == PalicoEquipmentType.Weapon)
        {
            foreach (PalicoWeapon weapon in PalicoEquipmentManager.CraftedWeapons)
            {
                AddEquipmentOption(weapon);
            }
        }
        else if (equipmentType == PalicoEquipmentType.Head)
        {
            List<PalicoArmor> headArmor = PalicoEquipmentManager.CraftedArmor.FindAll(armor => armor.Type == PalicoEquipmentType.Head);
            foreach (PalicoArmor armorPiece in headArmor)
            {
                AddEquipmentOption(armorPiece);
            }
        }
        else if (equipmentType == PalicoEquipmentType.Chest)
        {
            List<PalicoArmor> chestArmor = PalicoEquipmentManager.CraftedArmor.FindAll(armor => armor.Type == PalicoEquipmentType.Chest);
            foreach (PalicoArmor armorPiece in chestArmor)
            {
                AddEquipmentOption(armorPiece);
            }
        }
    }

    private void AddEquipmentOption(Equipment equipment)
    {
        EquipmentSelectionButton equipmentSelectionButton = MonsterHunterIdle.PackedScenes.GetEquipmentInfoButton(equipment);
        _equipmentSelectionButtons.Add(equipmentSelectionButton);
        _equipmentSelectionButtonContainer.AddChild(equipmentSelectionButton);
        equipmentSelectionButton.Pressed += () =>
        {
            ShowEquipmentPopup(equipmentSelectionButton.Equipment);

            // Unpress the other buttons   
            List<EquipmentSelectionButton> otherButtons = _equipmentSelectionButtons.FindAll(button => button != equipmentSelectionButton);
            foreach (EquipmentSelectionButton otherButton in otherButtons)
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
    }

    private void ShowEquipmentPopup(Equipment equipment)
    {
        EquipmentInfoPopup equipmentInfoPopup = MonsterHunterIdle.PackedScenes.GetEquipmentInfoPopup(equipment);
        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.Popup, equipmentInfoPopup);
    }
}

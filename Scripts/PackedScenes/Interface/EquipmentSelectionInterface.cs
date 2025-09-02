using Godot;
using System.Collections.Generic;
using System.Reflection;

namespace MonsterHunterIdle;

public partial class EquipmentSelectionInterface : NinePatchRect
{
    [Export]
    private Container _equipmentSelectionButtonContainer;

    private List<EquipmentSelectionButton> _equipmentSelectionButtons = new List<EquipmentSelectionButton>();
    private EquipmentInfoPopup _equipmentInfoPopup;

    private Equipment _equipment;
    private Palico _palico;

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
    
    private void OnChangePalicoEquipmentButtonPressed(Palico palico, PalicoEquipmentType equipmentType)
    {
        QueueFree();
    }
    // * END - Signal Methods

    // Add equipment buttons that correspond to the equipment type
    // For Hunter
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
    }

    // For Palicos
    public void SetEquipment(Palico palico, PalicoEquipmentType equipmentType)
    {
        _palico = palico;

        if (equipmentType == PalicoEquipmentType.Weapon)
        {
            for (int equipmentIndex = 0; equipmentIndex < PalicoEquipmentManager.CraftedWeapons.Count; equipmentIndex++)
            {
                PalicoWeapon weapon = PalicoEquipmentManager.CraftedWeapons[equipmentIndex];
                AddEquipmentOption(weapon, equipmentIndex);
            }
        }
        else if (equipmentType == PalicoEquipmentType.Head)
        {
            List<PalicoArmor> headArmor = PalicoEquipmentManager.CraftedArmor.FindAll(armor => armor.Type == PalicoEquipmentType.Head);
            for (int equipmentIndex = 0; equipmentIndex < headArmor.Count; equipmentIndex++)
            {
                PalicoArmor headArmorPiece = headArmor[equipmentIndex];
                AddEquipmentOption(headArmorPiece, equipmentIndex);
            }
        }
        else if (equipmentType == PalicoEquipmentType.Chest)
        {
            List<PalicoArmor> chestArmor = PalicoEquipmentManager.CraftedArmor.FindAll(armor => armor.Type == PalicoEquipmentType.Chest);
            for (int equipmentIndex = 0; equipmentIndex < chestArmor.Count; equipmentIndex++)
            {
                PalicoArmor chestArmorPiece = chestArmor[equipmentIndex];
                AddEquipmentOption(chestArmorPiece, equipmentIndex);
            }
        }
    }

    private void AddEquipmentOption(Equipment equipment, int index = -1)
    {
        EquipmentSelectionButton equipmentSelectionButton = MonsterHunterIdle.PackedScenes.GetEquipmentInfoButton(equipment, index);
        _equipmentSelectionButtons.Add(equipmentSelectionButton);
        _equipmentSelectionButtonContainer.AddChild(equipmentSelectionButton);
        equipmentSelectionButton.Pressed += () =>
        {
            ShowEquipmentPopup(equipmentSelectionButton.Equipment, index);

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
        int positionIndex = IsInstanceValid(_equipmentInfoPopup) ? 2 : 1;
        if (IsInstanceValid(_equipmentInfoPopup)) _equipmentInfoPopup.QueueFree();

        _equipmentInfoPopup = MonsterHunterIdle.PackedScenes.GetEquipmentInfoPopup(equipment);
        Container interfaceContainer = GetParentOrNull<Container>();
        AddSibling(_equipmentInfoPopup);
        interfaceContainer.MoveChild(_equipmentInfoPopup, positionIndex); // Move the node behind the monster interface
    }

    private void ShowEquipmentPopup(Equipment equipment, int index)
    {
        Control popup;
        if (equipment is not PalicoEquipment palicoEquipment)
        {
            popup = MonsterHunterIdle.PackedScenes.GetEquipmentInfoPopup(equipment);
        }
        else 
        {
            popup = MonsterHunterIdle.PackedScenes.GetPalicoEquipmentInfoPopup(_palico, palicoEquipment, index);
        }

        if (popup == null)
        {
            string className = MethodBase.GetCurrentMethod().DeclaringType.Name;
            string message = $"Couldn't Show Popup For Equipment: {equipment.Name}";
            PrintRich.PrintError(className, message);

            return;
        }

        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.Popup, popup);
    }
}

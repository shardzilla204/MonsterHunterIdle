using Godot;

namespace MonsterHunterIdle;

public partial class EquipmentInfoPopup : NinePatchRect
{
    [Export]
    private Container _infoContainer;

    [Export]
    private CustomButton _cancelButton;

    [Export]
    private CustomButton _supplyButton; // Equip/Unequip

    private Equipment _equipment;
    private bool _isEquipped;

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.ChangeEquipmentButtonPressed -= OnChangeEquipmentButtonPressed;
        MonsterHunterIdle.Signals.InterfaceChanged -= OnInterfaceChanged;
        MonsterHunterIdle.Signals.Popup -= OnPopup;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.ChangeEquipmentButtonPressed += OnChangeEquipmentButtonPressed;
        MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
        MonsterHunterIdle.Signals.Popup += OnPopup;
    }

    public override void _Ready()
    {
        _cancelButton.Pressed += QueueFree;
        _supplyButton.Pressed += OnSupplyButtonPressed;
    }

    // * START - Signal Methods
   

    private void OnChangeEquipmentButtonPressed(Equipment equipment)
    {
        QueueFree();
    }

    private void OnInterfaceChanged(InterfaceType interfaceType)
    {
        QueueFree();
    }

    private void OnPopup(Node node)
    {
        QueueFree();
    }

    private void OnSupplyButtonPressed()
    {
        Equipment equipment;
        if (!_isEquipped)
        {
            HunterManager.Equip(_equipment);
            equipment = EquipEquipment();
        }
        else
        {
            HunterManager.Unequip(_equipment);
            equipment = UnequipEquipment();
        }
        SetSupplyButtonText(equipment);

        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.EquipmentChanged);
    }
    // * END - Signal Methods

    /// Set the equipment with <see cref="_equipment"/> to the corresponding equipment slot
    private Equipment EquipEquipment()
    {
        Equipment equipment = null;

        if (_equipment is Weapon weapon)
        {
            Hunter.Weapon = weapon;
            equipment = Hunter.Weapon;
        }
        else if (_equipment is Armor armor)
        {
            switch (armor.Category)
            {
                case ArmorCategory.Head:
                    Hunter.Head = armor;
                    equipment = Hunter.Head;
                    break;
                case ArmorCategory.Chest:
                    Hunter.Chest = armor;
                    equipment = Hunter.Chest;
                    break;
                case ArmorCategory.Arm:
                    Hunter.Arm = armor;
                    equipment = Hunter.Arm;
                    break;
                case ArmorCategory.Waist:
                    Hunter.Waist = armor;
                    equipment = Hunter.Waist;
                    break;
                case ArmorCategory.Leg:
                    Hunter.Leg = armor;
                    equipment = Hunter.Leg;
                    break;
            }
        }
        return equipment;
    }

    // Create an empty equipment object
    private Equipment UnequipEquipment()
    {
        Equipment equipment = null;

        if (_equipment is Weapon)
        {
            Hunter.Weapon = new Weapon();
            equipment = Hunter.Weapon;
        }
        else if (_equipment is Armor armor)
        {
            switch (armor.Category)
            {
                case ArmorCategory.Head:
                    Hunter.Head = new Armor(ArmorCategory.Head);
                    equipment = Hunter.Head;
                    break;
                case ArmorCategory.Chest:
                    Hunter.Chest = new Armor(ArmorCategory.Chest);
                    equipment = Hunter.Chest;
                    break;
                case ArmorCategory.Arm:
                    Hunter.Arm = new Armor(ArmorCategory.Arm);
                    equipment = Hunter.Arm;
                    break;
                case ArmorCategory.Waist:
                    Hunter.Waist = new Armor(ArmorCategory.Waist);
                    equipment = Hunter.Waist;
                    break;
                case ArmorCategory.Leg:
                    Hunter.Leg = new Armor(ArmorCategory.Leg);
                    equipment = Hunter.Leg;
                    break;
            }
        }
        return equipment;
    }

    public void SetEquipment(Equipment equipment)
    {
        _equipment = equipment;

        // Add the equipment info node
        Texture2D equipmentIcon = MonsterHunterIdle.GetEquipmentIcon(equipment);
        string subGrade = equipment.SubGrade == 0 ? "" : $" (+{equipment.SubGrade})";
        HBoxContainer infoNode = Scenes.GetInfoNode(equipmentIcon, $"{equipment.Name}{subGrade}");
        _infoContainer.AddChild(infoNode);
        
        // Add info nodes depending on equipment
        if (equipment is Weapon weapon)
        {
            AddStatInfoNode(StatType.Attack, weapon.Attack);
            AddStatInfoNode(StatType.Affinity, weapon.Affinity);

            // Add special type info node
            if (weapon.Special != SpecialType.None)
            {
                Texture2D specialIcon = MonsterHunterIdle.GetSpecialTypeIcon(weapon.Special);
                HBoxContainer specialIconNode = Scenes.GetInfoNode(specialIcon, $"{weapon.SpecialAttack}");
                _infoContainer.AddChild(specialIconNode);
            }
        }
        else if (equipment is Armor armor)
        {
            AddStatInfoNode(StatType.Defense, armor.Defense);
        }

        SetSupplyButtonText(equipment);
    }

    private void AddStatInfoNode(StatType statType, int value)
    {
        Texture2D statTypeIcon = MonsterHunterIdle.GetStatTypeIcon(statType);
        HBoxContainer infoNode = Scenes.GetInfoNode(statTypeIcon, $"{value}");
        _infoContainer.AddChild(infoNode);
    }

    // Set the text depending if the equipment is equipped
    private void SetSupplyButtonText(Equipment equipment)
    {
        _isEquipped = HunterManager.IsEquipped(equipment);
        _supplyButton.Text = _isEquipped ? "Unequip" : "Equip";
    }
}

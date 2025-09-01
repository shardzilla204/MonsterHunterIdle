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
        MonsterHunterIdle.Signals.InterfaceChanged -= OnInterfaceChanged;
        MonsterHunterIdle.Signals.Popup -= OnPopup;
        MonsterHunterIdle.Signals.ChangeEquipmentButtonPressed -= OnChangeEquipmentButtonPressed;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
        MonsterHunterIdle.Signals.Popup += OnPopup;
        MonsterHunterIdle.Signals.ChangeEquipmentButtonPressed += OnChangeEquipmentButtonPressed;
    }

    public override void _Ready()
    {
        _cancelButton.Pressed += QueueFree;
        _supplyButton.Pressed += OnSupplyButtonPressed;
    }

    private void OnSupplyButtonPressed()
    {
        Equipment equipment;
        if (!_isEquipped)
        {
            equipment = EquipEquipment();
        }
        else
        {
            equipment = UnequipEquipment();
        }
        SetSupplyButtonText(equipment);

        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.EquipmentChanged, equipment);
    }

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

        Texture2D equipmentIcon = MonsterHunterIdle.GetEquipmentIcon(equipment);
        string subGrade = equipment.SubGrade == 0 ? "" : $" (+{equipment.SubGrade})";
        HBoxContainer infoNode = Scenes.GetInfoNode(equipmentIcon, $"{equipment.Name}{subGrade}");
        _infoContainer.AddChild(infoNode);

        if (equipment is Weapon weapon)
        {
            Texture2D attackIcon = MonsterHunterIdle.GetStatTypeIcon(StatType.Attack);
            HBoxContainer attackInfoNode = Scenes.GetInfoNode(attackIcon, $"{weapon.Attack}");
            _infoContainer.AddChild(attackInfoNode);

            Texture2D affinityIcon = MonsterHunterIdle.GetStatTypeIcon(StatType.Affinity);
            HBoxContainer affinityInfoNode = Scenes.GetInfoNode(affinityIcon, $"{weapon.Affinity}%");
            _infoContainer.AddChild(affinityInfoNode);

            if (weapon.Special != SpecialType.None)
            {
                Texture2D specialIcon = MonsterHunterIdle.GetSpecialTypeIcon(weapon.Special);
                HBoxContainer specialIconNode = Scenes.GetInfoNode(specialIcon, $"{weapon.SpecialAttack}");
                _infoContainer.AddChild(specialIconNode);
            }
        }
        else if (equipment is Armor armor)
        {
            Texture2D defenseIcon = MonsterHunterIdle.GetStatTypeIcon(StatType.Defense);
            HBoxContainer defenseInfoNode = Scenes.GetInfoNode(defenseIcon, $"{armor.Defense}");
            _infoContainer.AddChild(defenseInfoNode);
        }

        SetSupplyButtonText(equipment);
    }

    private void SetSupplyButtonText(Equipment equipment)
    {
        _isEquipped = HunterManager.IsEquipped(equipment);
        _supplyButton.Text = _isEquipped ? "Unequip" : "Equip";
    }

    private void OnInterfaceChanged(InterfaceType interfaceType)
    {
        QueueFree();
    }
    
    private void OnChangeEquipmentButtonPressed(Equipment equipment)
    {
        QueueFree();
    }

    private void OnPopup(Node node)
    {
        QueueFree();
    }
}

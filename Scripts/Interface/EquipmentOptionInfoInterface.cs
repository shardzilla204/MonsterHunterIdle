using Godot;

namespace MonsterHunterIdle;

public partial class EquipmentOptionInfoInterface : NinePatchRect
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
        Hunter hunter = MonsterHunterIdle.HunterManager.Hunter;

        if (_equipment is Weapon weapon)
        {
            hunter.Weapon = weapon;
            equipment = hunter.Weapon;
        }
        else if (_equipment is Armor armor)
        {
            switch (armor.Category)
            {
                case ArmorCategory.Head:
                    hunter.Head = armor;
                    equipment = hunter.Head;
                    break;
                case ArmorCategory.Chest:
                    hunter.Chest = armor;
                    equipment = hunter.Chest;
                    break;
                case ArmorCategory.Arm:
                    hunter.Arm = armor;
                    equipment = hunter.Arm;
                    break;
                case ArmorCategory.Waist:
                    hunter.Waist = armor;
                    equipment = hunter.Waist;
                    break;
                case ArmorCategory.Leg:
                    hunter.Leg = armor;
                    equipment = hunter.Leg;
                    break;
            }
        }
        return equipment;
    }

    // Create an empty equipment object
    private Equipment UnequipEquipment()
    {
        Equipment equipment = null;
        Hunter hunter = MonsterHunterIdle.HunterManager.Hunter;

        if (_equipment is Weapon)
        {
            hunter.Weapon = new Weapon();
            equipment = hunter.Weapon;
        }
        else if (_equipment is Armor armor)
        {
            switch (armor.Category)
            {
                case ArmorCategory.Head:
                    hunter.Head = new Armor(ArmorCategory.Head);
                    equipment = hunter.Head;
                    break;
                case ArmorCategory.Chest:
                    hunter.Chest = new Armor(ArmorCategory.Chest);
                    equipment = hunter.Chest;
                    break;
                case ArmorCategory.Arm:
                    hunter.Arm = new Armor(ArmorCategory.Arm);
                    equipment = hunter.Arm;
                    break;
                case ArmorCategory.Waist:
                    hunter.Waist = new Armor(ArmorCategory.Waist);
                    equipment = hunter.Waist;
                    break;
                case ArmorCategory.Leg:
                    hunter.Leg = new Armor(ArmorCategory.Leg);
                    equipment = hunter.Leg;
                    break;
            }
        }
        return equipment;
    }

    public void SetEquipment(Equipment equipment)
    {
        _equipment = equipment; 

        Texture2D equipmentIcon = MonsterHunterIdle.GetEquipmentIcon(equipment);
        HBoxContainer infoNode = GetInfoNode(equipmentIcon, equipment.Name);
        _infoContainer.AddChild(infoNode);

        if (equipment is Weapon weapon)
        {
            Texture2D attackIcon = MonsterHunterIdle.GetStatIcon(StatType.Attack);
            HBoxContainer attackInfoNode = GetInfoNode(attackIcon, $"{weapon.Attack}");
            _infoContainer.AddChild(attackInfoNode);

            Texture2D affinityIcon = MonsterHunterIdle.GetStatIcon(StatType.Affinity);
            HBoxContainer affinityInfoNode = GetInfoNode(affinityIcon, $"{weapon.Affinity}%");
            _infoContainer.AddChild(affinityInfoNode);
        }
        else if (equipment is Armor armor)
        {
            Texture2D defenseIcon = MonsterHunterIdle.GetStatIcon(StatType.Defense);
            HBoxContainer defenseInfoNode = GetInfoNode(defenseIcon, $"{armor.Defense}");
            _infoContainer.AddChild(defenseInfoNode);
        }

        SetSupplyButtonText(equipment);
    }

    private HBoxContainer GetInfoNode(Texture2D texture, string text)
    {
        HBoxContainer infoNode = new HBoxContainer();

        int size = 40;
        TextureRect iconTextureRect = new TextureRect()
        {
            Texture = texture,
            CustomMinimumSize = new Vector2(size, size),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered
        };
        infoNode.AddChild(iconTextureRect);

        Label nameLabel = new Label()
        {
            Text = text,
            HorizontalAlignment = HorizontalAlignment.Right,
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        int fontSize = 20;
        nameLabel.AddThemeFontSizeOverride("font_size", fontSize);
        infoNode.AddChild(nameLabel);

        return infoNode;
    }

    private void SetSupplyButtonText(Equipment equipment)
    {
        _isEquipped = MonsterHunterIdle.HunterManager.IsEquipped(equipment);
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

using Godot;

namespace MonsterHunterIdle;

public partial class PalicoEquipmentInfoPopup : NinePatchRect
{
    [Export]
    private Container _infoContainer;

    [Export]
    private CustomButton _cancelButton;

    [Export]
    private CustomButton _supplyButton; // Equip/Unequip

    private Palico _palico;
    private PalicoEquipment _equipment;
    private bool _isEquipped;

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.ChangePalicoEquipmentButtonPressed -= OnChangeEquipmentButtonPressed;
        MonsterHunterIdle.Signals.Popup -= OnPopup;
        MonsterHunterIdle.Signals.InterfaceChanged -= OnInterfaceChanged;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.ChangePalicoEquipmentButtonPressed += OnChangeEquipmentButtonPressed;
        MonsterHunterIdle.Signals.Popup += OnPopup;
        MonsterHunterIdle.Signals.InterfaceChanged += OnInterfaceChanged;
    }

    public override void _Ready()
    {
        _cancelButton.Pressed += QueueFree;
        _supplyButton.Pressed += OnSupplyButtonPressed;
    }

    // * START - Signal Methods
    private void OnInterfaceChanged(InterfaceType interfaceType)
    {
        QueueFree();
    }
    
    private void OnChangeEquipmentButtonPressed(PalicoEquipmentType equipmentType)
    {
        QueueFree();
    }

    private void OnPopup(Node node)
    {
        QueueFree();
    }

    private void OnSupplyButtonPressed()
    {
        if (!_isEquipped)
        {
            PalicoManager.Equip(_palico, _equipment);
        }
        else
        {
            PalicoManager.Unequip(_palico, _equipment);
        }
        SetSupplyButtonText(_equipment);

        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoEquipmentChanged, _equipment);
    }
    // * END - Signal Methods

    // Create an empty equipment object
    private PalicoEquipment UnequipEquipment()
    {
        PalicoEquipment equipment = null;

        if (_equipment is PalicoWeapon)
        {
            _palico.Weapon = new PalicoWeapon();
            equipment = _palico.Weapon;
        }
        else if (_equipment is PalicoArmor armor)
        {
            switch (armor.Type)
            {
                case PalicoEquipmentType.Head:
                    _palico.Head = armor;
                    break;
                case PalicoEquipmentType.Chest:
                    _palico.Chest = armor;
                    break;
            }
        }
        return equipment;
    }

    public void SetEquipment(Palico palico, PalicoEquipment equipment)
    {
        _palico = palico;
        _equipment = equipment;

        Texture2D equipmentIcon = MonsterHunterIdle.GetEquipmentIcon(equipment);
        string subGrade = equipment.SubGrade == 0 ? "" : $" (+{equipment.SubGrade})";
        HBoxContainer infoNode = Scenes.GetInfoNode(equipmentIcon, $"{equipment.Name}{subGrade}");
        _infoContainer.AddChild(infoNode);

        if (equipment is PalicoWeapon palicoWeapon)
        {
            Texture2D attackIcon = MonsterHunterIdle.GetStatTypeIcon(StatType.Attack);
            HBoxContainer attackInfoNode = Scenes.GetInfoNode(attackIcon, $"{palicoWeapon.Attack}");
            _infoContainer.AddChild(attackInfoNode);

            Texture2D affinityIcon = MonsterHunterIdle.GetStatTypeIcon(StatType.Affinity);
            HBoxContainer affinityInfoNode = Scenes.GetInfoNode(affinityIcon, $"{palicoWeapon.Affinity}%");
            _infoContainer.AddChild(affinityInfoNode);

            if (palicoWeapon.Special != SpecialType.None)
            {
                Texture2D specialIcon = MonsterHunterIdle.GetSpecialTypeIcon(palicoWeapon.Special);
                HBoxContainer specialIconNode = Scenes.GetInfoNode(specialIcon, $"{palicoWeapon.SpecialAttack}");
                _infoContainer.AddChild(specialIconNode);
            }
        }
        else if (equipment is PalicoArmor palicoArmor)
        {
            Texture2D defenseIcon = MonsterHunterIdle.GetStatTypeIcon(StatType.Defense);
            HBoxContainer defenseInfoNode = Scenes.GetInfoNode(defenseIcon, $"{palicoArmor.Defense}");
            _infoContainer.AddChild(defenseInfoNode);
        }

        SetSupplyButtonText(equipment);
    }

    private void SetSupplyButtonText(PalicoEquipment equipment)
    {
        bool isEquipped = PalicoManager.IsEquipped(_palico, equipment);
        _supplyButton.Text = isEquipped ? "Unequip" : "Equip";
    }
}

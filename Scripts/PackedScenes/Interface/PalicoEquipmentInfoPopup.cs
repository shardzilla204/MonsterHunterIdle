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
    private int _equipmentIndex;

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
    
    private void OnChangeEquipmentButtonPressed(Palico palico, PalicoEquipmentType equipmentType)
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

        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoEquipmentChanged, _palico);

        QueueFree();
    }
    // * END - Signal Methods

    // Set the palico 
    public void SetEquipment(Palico palico, PalicoEquipment equipment, int index)
    {
        _palico = palico;
        _equipment = equipment;
        _equipmentIndex = index;

        Texture2D equipmentIcon = MonsterHunterIdle.GetEquipmentIcon(equipment);
        string subGrade = equipment.SubGrade == 0 ? "" : $" (+{equipment.SubGrade})";
        HBoxContainer infoNode = Scenes.GetInfoNode(equipmentIcon, $"{equipment.Name}{subGrade}");
        _infoContainer.AddChild(infoNode);

        if (equipment is PalicoWeapon weapon)
        {
            AddStatInfoNode(StatType.Attack, weapon.Attack);
            AddStatInfoNode(StatType.Affinity, weapon.Affinity);

            if (weapon.Special != SpecialType.None)
            {
                Texture2D specialIcon = MonsterHunterIdle.GetSpecialTypeIcon(weapon.Special);
                HBoxContainer specialIconNode = Scenes.GetInfoNode(specialIcon, $"{weapon.SpecialAttack}");
                _infoContainer.AddChild(specialIconNode);
            }
        }
        else if (equipment is PalicoArmor armor)
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
    private void SetSupplyButtonText(PalicoEquipment equipment)
    {
        bool isEquipped = PalicoManager.IsEquipped(_palico, equipment, _equipmentIndex);
        _supplyButton.Text = isEquipped ? "Unequip" : "Equip";
    }
}

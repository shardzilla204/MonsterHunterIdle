using Godot;

namespace MonsterHunterIdle;

public partial class EquipmentInfo : HBoxContainer
{
    [Export]
    private TextureRect _iconTextureRect;

    [Export]
    private CustomButton _changeEquipmentButton;

    private Equipment _equipment;

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.EquipmentChanged -= OnEquipmentChanged;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.EquipmentChanged += OnEquipmentChanged;
    }

    public override void _Ready()
    {
        _changeEquipmentButton.Pressed += OnChangeEquipmentButtonPressed;
    }

    // * START - Signal Methods
    private void OnChangeEquipmentButtonPressed()
    {
        if (_equipment == null) return;
        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.ChangeEquipmentButtonPressed, _equipment);
    }

    // Update the info displayed
    private void OnEquipmentChanged()
    {
        SetInfo(Hunter.Weapon);
        SetInfo(Hunter.Head);
        SetInfo(Hunter.Chest);
        SetInfo(Hunter.Arm);
        SetInfo(Hunter.Waist);
        SetInfo(Hunter.Leg);
    }
    // * END - Signal Methods

    /// Equipment will never be null, but empty | <see cref="Hunter"/>
    public void SetEquipment(Equipment equipment)
    {
        GD.Print($"Name: {equipment.Name}");
        if (equipment is Weapon weapon)
        {
            _equipment = weapon.Tree == WeaponTree.None ? null : equipment;
        }
        else if (equipment is Armor armor)
        {
            _equipment = armor.Set == ArmorSet.None ? null : equipment;
        }
        SetInfo(equipment);
    }

    private void SetInfo(Equipment equipment)
    {
        _iconTextureRect.Texture = MonsterHunterIdle.GetEquipmentIcon(equipment);

        if (equipment is Weapon weapon)
        {
            string subGrade = equipment.SubGrade == 0 ? "" : $" (+{equipment.SubGrade})";
            _changeEquipmentButton.Text = weapon.Tree == WeaponTree.None ? "None" : $"{equipment.Name}{subGrade}";
        }
        else if (equipment is Armor armor)
        {
            string subGrade = equipment.SubGrade == 0 ? "" : $" (+{equipment.SubGrade})";
            _changeEquipmentButton.Text = armor.Set == ArmorSet.None ? "None" : $"{equipment.Name}{subGrade}";
        }
    }
}

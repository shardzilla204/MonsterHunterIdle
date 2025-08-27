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
        MonsterHunterIdle.Signals.EquipmentChanged -= EquipmentChanged;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.EquipmentChanged += EquipmentChanged;
    }

    public override void _Ready()
    {
        _changeEquipmentButton.Pressed += ChangeEquipmentButtonPressed;
    }

    /// Equipment will never be null, but empty | <see cref="Hunter"/>
    public void SetEquipment(Equipment equipment)
    {
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

    private void ChangeEquipmentButtonPressed()
    {
        if (_equipment == null) return;
        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.ChangeEquipmentButtonPressed, _equipment);
    }

    private void EquipmentChanged(Equipment equipment)
    {
        Hunter hunter = MonsterHunterIdle.HunterManager.Hunter;
        if (_equipment is Weapon && equipment is Weapon)
        {
            SetInfo(hunter.Weapon);
        }
        else if (_equipment is Armor && equipment is Armor newArmor)
        {
            switch (newArmor.Category)
            {
                case ArmorCategory.Head:
                    SetInfo(hunter.Head);
                    break;
                case ArmorCategory.Chest:
                    SetInfo(hunter.Chest);
                    break;
                case ArmorCategory.Arm:
                    SetInfo(hunter.Arm);
                    break;
                case ArmorCategory.Waist:
                    SetInfo(hunter.Waist);
                    break;
                case ArmorCategory.Leg:
                    SetInfo(hunter.Leg);
                    break;
            }
        }
    }
}

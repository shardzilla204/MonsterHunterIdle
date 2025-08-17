using Godot;

namespace MonsterHunterIdle;

public partial class ChangeEquipmentInterface : NinePatchRect
{
    [Export]
    private Label _changeLabel;

    [Export]
    private CustomButton _yesButton;

    [Export]
    private CustomButton _noButton;

    private Equipment _equipment;

    public override void _Ready()
    {
        _yesButton.Pressed += OnYesButtonPressed;
        _noButton.Pressed += OnNoButtonPressed;
    }

    public void SetEquipment(Equipment equipment)
    {
        _equipment = equipment;

        string changeMessage = $"Would you like to equip {equipment.Name}?";
        _changeLabel.Text = changeMessage;
    }

    private void OnYesButtonPressed()
    {
        Hunter hunter = MonsterHunterIdle.HunterManager.Hunter;
        if (_equipment is Weapon weapon)
        {
            Weapon oldWeapon = hunter.Weapon;
            hunter.Weapon = weapon;

            if (oldWeapon != null)
            {
                // Console message
                string weaponChangedMessage = $"{oldWeapon.Name} has been swapped out for {weapon.Name}";
                PrintRich.Print(TextColor.Orange, weaponChangedMessage);
            }
        }
        else if (_equipment is Armor armor)
        {
            Armor oldArmor = null;
            switch (armor.Category)
            {
                case ArmorCategory.Head:
                    oldArmor = hunter.Head;
                    hunter.Head = armor;
                    break;
                case ArmorCategory.Chest:
                    oldArmor = hunter.Chest;
                    hunter.Chest = armor;
                    break;
                case ArmorCategory.Arm:
                    oldArmor = hunter.Arm;
                    hunter.Arm = armor;
                    break;
                case ArmorCategory.Waist:
                    oldArmor = hunter.Waist;
                    hunter.Waist = armor;
                    break;
                case ArmorCategory.Leg:
                    oldArmor = hunter.Leg;
                    hunter.Leg = armor;
                    break;
            }

            if (oldArmor != null)
            {
                // Console message
                string armorChangedMessage = $"{oldArmor.Name} has been swapped out for {armor.Name}";
                PrintRich.PrintLine(TextColor.Orange, armorChangedMessage);
            }
        }

        QueueFree();
        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.EquipmentChanged, _equipment);
    }

    private void OnNoButtonPressed()
    {
        QueueFree();
        MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.EquipmentChanged, _equipment);
    }
}

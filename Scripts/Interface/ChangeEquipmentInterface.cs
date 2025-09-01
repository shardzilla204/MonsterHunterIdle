using Godot;

namespace MonsterHunterIdle;

public partial class ChangeEquipmentInterface : NinePatchRect
{
    [Signal]
    public delegate void FinishedEventHandler(Equipment equipment);

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
        _noButton.Pressed += QueueFree;

        TreeExiting += () => EmitSignal(SignalName.Finished, _equipment);
    }

    public void SetEquipment(Equipment equipment)
    {
        _equipment = equipment;

        string changeMessage = $"Would you like to equip {equipment.Name}?";
        _changeLabel.Text = changeMessage;
    }

    private void OnYesButtonPressed()
    {
        if (_equipment is Weapon weapon)
        {
            Weapon oldWeapon = Hunter.Weapon;
            Hunter.Weapon = weapon;

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
                    oldArmor = Hunter.Head;
                    Hunter.Head = armor;
                    break;
                case ArmorCategory.Chest:
                    oldArmor = Hunter.Chest;
                    Hunter.Chest = armor;
                    break;
                case ArmorCategory.Arm:
                    oldArmor = Hunter.Arm;
                    Hunter.Arm = armor;
                    break;
                case ArmorCategory.Waist:
                    oldArmor = Hunter.Waist;
                    Hunter.Waist = armor;
                    break;
                case ArmorCategory.Leg:
                    oldArmor = Hunter.Leg;
                    Hunter.Leg = armor;
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
    }
}

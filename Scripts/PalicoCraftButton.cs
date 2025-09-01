using Godot;

namespace MonsterHunterIdle;

public partial class PalicoCraftButton : CustomButton
{
    [Export]
    private TextureRect _equipmentIcon;

    [Export]
    private Label _equipmentName;

    [Export]
    private Label _equipmentAmount;

    private PalicoEquipment _equipment;

    public override void _Ready()
    {
        base._Ready();
        Pressed += () => MonsterHunterIdle.Signals.EmitSignal(Signals.SignalName.PalicoCraftButtonPressed, _equipment);
    }

    public void SetEquipment(PalicoEquipment equipment)
    {
        if (equipment == null) return;

        int equipmentAmount = PalicoEquipmentManager.GetEquipmentAmount(equipment);
        _equipmentAmount.Text = $"{equipmentAmount}x";

        bool canCraft = PalicoEquipmentManager.CanCraft(equipment);
        _equipmentAmount.SelfModulate = canCraft ? Colors.White : Colors.SeaGreen;

        _equipmentName.Text = $"{equipment.Name}";

        Texture2D equipmentIcon = null;
        string gradeWhiteString = MonsterHunterIdle.GetGradeColorString(0);
        if (equipment is PalicoWeapon weapon)
        {
            string filePath = $"res://Assets/Images/Palico/Palico{weapon.Type}{gradeWhiteString}.png";
            equipmentIcon = MonsterHunterIdle.GetTexture(filePath);
        }
        else if (equipment is PalicoArmor armor)
        {
            string filePath = $"res://Assets/Images/Palico/Palico{armor.Type}{gradeWhiteString}.png";
            equipmentIcon = MonsterHunterIdle.GetTexture(filePath);
        }

        _equipmentIcon.Texture = equipmentIcon == null ? null : equipmentIcon;

        _equipment = equipment;
	}
}

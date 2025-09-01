using Godot;

namespace MonsterHunterIdle;

public partial class PalicoCraftOptionButton : CustomButton
{
    [Export]
    private TextureRect _iconTextureRect;

    public void SetEquipment(PalicoEquipment equipment)
    {
        Texture2D equipmentIcon = MonsterHunterIdle.GetEquipmentIcon(equipment);
        _iconTextureRect.Texture = equipmentIcon;

        TooltipText = $"(+ {equipment.SubGrade})";
    }
}

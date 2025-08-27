using Godot;

namespace MonsterHunterIdle;

public partial class EquipmentInfoButton : CustomButton
{
    [Export]
    private TextureRect _iconTextureRect;

    public Equipment Equipment;

    public void SetEquipment(Equipment equipment)
    {
        Equipment = equipment;

        Texture2D equipmentIcon = MonsterHunterIdle.GetEquipmentIcon(equipment);
        _iconTextureRect.Texture = equipmentIcon;
    }
}

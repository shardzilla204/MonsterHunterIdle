using Godot;

namespace MonsterHunterIdle;

// TODO: Change the icon color depending on the rarity
public partial class EquipmentOptionButton : CustomButton
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

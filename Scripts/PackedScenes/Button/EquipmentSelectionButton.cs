using Godot;

namespace MonsterHunterIdle;

public partial class EquipmentSelectionButton : CustomButton
{
    [Export]
    private TextureRect _iconTextureRect;

    public Equipment Equipment;
    
    // Sets the texture
    public void SetEquipment(Equipment equipment)
    {
        Equipment = equipment;

        Texture2D equipmentIcon = MonsterHunterIdle.GetEquipmentIcon(equipment);
        _iconTextureRect.Texture = equipmentIcon;
    }
}

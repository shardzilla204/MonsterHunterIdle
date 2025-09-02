using Godot;

namespace MonsterHunterIdle;

public partial class EquipmentSelectionButton : CustomButton
{
    [Export]
    private TextureRect _iconTextureRect;

    public Equipment Equipment;
    public int EquipmentIndex = -1; // Mainly for palico equipment
    
    // Sets the texture
    public void SetEquipment(Equipment equipment, int index)
    {
        Equipment = equipment;
        EquipmentIndex = index;

        Texture2D equipmentIcon = MonsterHunterIdle.GetEquipmentIcon(equipment);
        _iconTextureRect.Texture = equipmentIcon;
    }
}

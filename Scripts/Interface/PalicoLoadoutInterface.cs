using System;
using Godot;

namespace MonsterHunterIdle;

public partial class PalicoLoadoutInterface : NinePatchRect
{
    [Export]
    private Label _palicoName;

    [Export]
    private Container _palicoEquipmentInfoContainer;

    public Palico Palico;

    public void SetPalico(Palico palico)
    {
        Palico = palico;

        _palicoName.Text = $"{palico.Name}'s Loadout";

        int enumCount = Enum.GetNames<PalicoEquipmentType>().Length - 1; // Don't count PalicoEquipmentType.None
        for (int enumIndex = 0; enumIndex < enumCount; enumIndex++)
        {
            PalicoEquipmentInfo palicoEquipmentInfo = MonsterHunterIdle.PackedScenes.GetPalicoEquipmentInfo(palico, (PalicoEquipmentType) enumIndex);
            _palicoEquipmentInfoContainer.AddChild(palicoEquipmentInfo);
        }
    }
}

using Godot;

namespace MonsterHunterIdle;

public partial class PalicoLoadout : NinePatchRect
{
    [Export]
    private Label _palicoName;

    [Export]
    private PalicoEquipmentInfo _weaponInfo;

    [Export]
    private PalicoEquipmentInfo _helmetInfo;

    [Export]
    private PalicoEquipmentInfo _armorInfo;

    public Palico Palico;

    public void SetPalico(Palico palico)
    {
        Palico = palico;

        _palicoName.Text = $"{palico.Name}'s Loadout";
        _weaponInfo.SetStats(palico);
        _helmetInfo.SetStats(palico);
        _armorInfo.SetStats(palico);
    }
}

using System;
using Godot;

namespace MonsterHunterIdle;

public partial class PalicoLoadoutInterface : NinePatchRect
{
    [Export]
    private Label _palicoName;

    [Export]
    private Container _palicoEquipmentInfoContainer;

    private Palico _palico = new Palico();

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.PalicoEquipmentChanged -= OnPalicoEquipmentChanged;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.PalicoEquipmentChanged += OnPalicoEquipmentChanged;
    }

    // * START - Signal Methods
    private void OnPalicoEquipmentChanged(Palico palico)
    {
        if (palico != _palico) return;

        RefreshEquipmentInfo();
    }
    // * END - Signal Methods

    public void SetPalico(Palico palico)
    {
        _palico = palico;

        _palicoName.Text = $"{palico.Name}'s Loadout";

        RefreshEquipmentInfo();
    }

    private void RefreshEquipmentInfo()
    {
        foreach (Node palicoEquipmentInfo in _palicoEquipmentInfoContainer.GetChildren())
        {
            palicoEquipmentInfo.QueueFree();
        }

        int enumCount = Enum.GetNames<PalicoEquipmentType>().Length - 1; // Don't count PalicoEquipmentType.None
        for (int enumIndex = 0; enumIndex < enumCount; enumIndex++)
        {
            PalicoEquipmentInfo palicoEquipmentInfo = MonsterHunterIdle.PackedScenes.GetPalicoEquipmentInfo(_palico, (PalicoEquipmentType)enumIndex);
            _palicoEquipmentInfoContainer.AddChild(palicoEquipmentInfo);
        }
    }
}

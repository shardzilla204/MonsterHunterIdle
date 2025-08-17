using Godot;

namespace MonsterHunterIdle;

public partial class LoadoutStats : NinePatchRect
{
    [Export]
    private Label _attackStatLabel;

    [Export]
    private Label _defenseStatLabel;

    [Export]
    private Label _affinityStatLabel;

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.EquipmentChanged -= EquipmentChanged;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.EquipmentChanged += EquipmentChanged;
    }

    public override void _Ready()
    {
        SetStats();
    }

    private void SetStats()
    {
        Hunter hunter = MonsterHunterIdle.HunterManager.Hunter;

        int hunterAttack = hunter.Weapon != null ? hunter.Weapon.Attack : 0;
        _attackStatLabel.Text = $"{hunterAttack}";

        int hunterDefense = GetHunterDefense();
        _defenseStatLabel.Text = $"{hunterDefense}";

        float hunterAffinity = hunter.Weapon != null ? hunter.Weapon.Affinity : 0;
        _affinityStatLabel.Text = $"{hunterAffinity}%";
    }

    // Go through all armor pieces and get their defense value
    private int GetHunterDefense()
    {
        int hunterDefense = 0;
        Hunter hunter = MonsterHunterIdle.HunterManager.Hunter;

        if (hunter.Head.Name != "") hunterDefense += hunter.Head.Defense;
        if (hunter.Chest.Name != "") hunterDefense += hunter.Chest.Defense;
        if (hunter.Arm.Name != "") hunterDefense += hunter.Arm.Defense;
        if (hunter.Waist.Name != "") hunterDefense += hunter.Waist.Defense;        
        if (hunter.Leg.Name != "") hunterDefense += hunter.Leg.Defense;  

        return hunterDefense;
    }

    private void EquipmentChanged(Equipment equipment)
    {
        SetStats();
    }
}

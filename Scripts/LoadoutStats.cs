using Godot;

namespace MonsterHunterIdle;

public partial class LoadoutStats : NinePatchRect
{
    [Export]
    private Container _statContainer;

    public override void _ExitTree()
    {
        MonsterHunterIdle.Signals.EquipmentChanged -= OnEquipmentChanged;
    }

    public override void _EnterTree()
    {
        MonsterHunterIdle.Signals.EquipmentChanged += OnEquipmentChanged;
    }

    public override void _Ready()
    {
        // Set initially
        OnEquipmentChanged();
    }

    // * START - Signal Methods
    private void OnEquipmentChanged()
    {
        ClearStats();
        SetStats();
    }
    // * END - Signal Methods

    private void SetStats()
    {
        int weaponAttack = Hunter.Weapon != null ? Hunter.Weapon.Attack : 0;
        HBoxContainer attackStat = Scenes.GetLoadoutStat(StatType.Attack, $"{weaponAttack}");
        _statContainer.AddChild(attackStat);

        int armorDefense = GetArmorDefense();
        HBoxContainer defenseStat = Scenes.GetLoadoutStat(StatType.Defense, $"{armorDefense}");
        _statContainer.AddChild(defenseStat);

        int weaponAffinity = Hunter.Weapon != null ? Hunter.Weapon.Affinity : 0;
        HBoxContainer affinityStat = Scenes.GetLoadoutStat(StatType.Affinity, $"{weaponAffinity}%");
        _statContainer.AddChild(affinityStat);

        if (Hunter.Weapon.Special != SpecialType.None && Hunter.Weapon.SpecialAttack != 0)
        {
            HBoxContainer specialAttackStat = Scenes.GetLoadoutStat(Hunter.Weapon.Special, $"{Hunter.Weapon.SpecialAttack}");
            _statContainer.AddChild(specialAttackStat);
        }
    }

    private void ClearStats()
    {
        foreach (Node loadoutStat in _statContainer.GetChildren())
        {
            loadoutStat.QueueFree();
        }
    }

    // Go through all armor pieces and get their defense value
    private int GetArmorDefense()
    {
        int hunterDefense = 0;

        if (Hunter.Head.Name != "") hunterDefense += Hunter.Head.Defense;
        if (Hunter.Chest.Name != "") hunterDefense += Hunter.Chest.Defense;
        if (Hunter.Arm.Name != "") hunterDefense += Hunter.Arm.Defense;
        if (Hunter.Waist.Name != "") hunterDefense += Hunter.Waist.Defense;
        if (Hunter.Leg.Name != "") hunterDefense += Hunter.Leg.Defense;

        return hunterDefense;
    }
}
